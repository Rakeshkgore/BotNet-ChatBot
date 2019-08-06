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
    public class OrderDialog : CancelAndHelpDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public OrderDialog(IConfiguration configuration, ILogger logger)
            : base(nameof(OrderDialog))
        {
            Configuration = configuration;
            Logger = logger;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new upsOrderNumberDialog(configuration, logger));
            AddDialog(new TrackingDialog(configuration, logger));
            AddDialog(new OrderNumberDialog(configuration, logger));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                TrackingTypeAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = (UserRequest)stepContext.Options;

            if (userInput.isOrder() == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What tracking number will you be entering?\n Say something like \"Order Number, Ups Order Number, Tracking Number\"")
                    }, cancellationToken);

            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Where is my order?\" or \"Inventory Details\"") }, cancellationToken);

            }
        }


        private async Task<DialogTurnResult> TrackingTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Result != null
                ?
                await LuisHelper.ExecuteLuisQuery(Configuration, Logger, stepContext.Context, cancellationToken)
                :
                new UserRequest();

            if (userInput.isUPSTrackingNumber == true)
            {
                return await stepContext.BeginDialogAsync(nameof(upsOrderNumberDialog), userInput, cancellationToken);
            }
            else if (userInput.isTrackingNumber == true)
            {
                return await stepContext.BeginDialogAsync(nameof(TrackingDialog), userInput, cancellationToken);
            }
            else if (userInput.isOrderNumber == true)
            {
                return await stepContext.BeginDialogAsync(nameof(OrderNumberDialog), userInput, cancellationToken);
            } else if (userInput.restart == true)
            {
                return await stepContext.BeginDialogAsync(nameof(UserDialog), userInput, cancellationToken);

            }else if (userInput.isjob == true) 
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please visit our jobs website to learn more : https://www.jobs-ups.com") }, cancellationToken);

            }
            else if (userInput.cancel == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for using the GLD Bot") }, cancellationToken);

            }
            else
            {
                return null;
            }

        }

    }


}
