using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CoreBot1.Dialogs
{
    public class InventoryDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public InventoryDialog(IConfiguration configuration, ILogger logger)
            : base(nameof(InventoryDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new SKUDialog(configuration, logger));
            AddDialog(new poNumberDialog(configuration, logger));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {

                IntroStepAsync,
                InventoryTypeAsync,

            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = (UserRequest)stepContext.Options;

            if (userInput.isInventory() == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What inventory number will you be entering?\n Say something like \"SKU# or PO# \"")
                    }, cancellationToken);

            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Where is my order?\" or \"inventory details\"") }, cancellationToken);

            }
        }
        private async Task<DialogTurnResult> InventoryTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Result != null
                ?
                await LuisHelper.ExecuteLuisQuery(Configuration, Logger, stepContext.Context, cancellationToken)
                :
                new UserRequest();

            if (userInput.isSKUNumber == true)
            {
                return await stepContext.BeginDialogAsync(nameof(SKUDialog), userInput, cancellationToken);
            }
            else if (userInput.ispurchaseOrderNumber == true)
            {
                return await stepContext.BeginDialogAsync(nameof(poNumberDialog), userInput, cancellationToken);
            }
            else if (userInput.cancel == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for using the GLD Bot") }, cancellationToken);

            }
            else if (userInput.restart == true)
            {
                return await stepContext.BeginDialogAsync(nameof(UserDialog), userInput, cancellationToken);

            }
            else if (userInput.isjob == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please visit our jobs website to learn more : https://www.jobs-ups.com") }, cancellationToken);

            }
            else
            {
                return null;
            }

        }

    }
}

