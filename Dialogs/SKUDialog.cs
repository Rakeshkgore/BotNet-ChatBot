using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CoreBot1.Dialogs
{
    public class SKUDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        public SKUDialog(IConfiguration configuration, ILogger logger)
            : base(nameof(SKUDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {

                GetInventoryNumberAsync,
                ProcessInventoryNumberAsync,
                NewTrackingNumber,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> GetInventoryNumberAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Inventory Number below:") }, cancellationToken);

        }

        private async Task<DialogTurnResult> ProcessInventoryNumberAsync(WaterfallStepContext stepContext,
           CancellationToken cancellationToken)
        {
            var userinput = stepContext.Result != null
                ?
            await LuisHelper.ExecuteLuisQuery(Configuration, Logger, stepContext.Context, cancellationToken)
                :
            new UserRequest();
            var userInput = new UserRequest();

            if (userinput.restart == true)
            {
                return await stepContext.BeginDialogAsync(nameof(UserDialog), userinput, cancellationToken);

            }else if(userinput.cancel == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for using the GLD Bot") }, cancellationToken);

            }else if (stepContext.Result is int || stepContext.Result is string)
            {
                userInput.SKUNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("  Invetory status of Sku# " + userInput.SKUNumber + ":" + "\nItem 1: 65 units\nLocation: Warehouse 1\nIf you would like to check inventory on another Sku# enter in the number? If not say \"no\"") }, cancellationToken);

            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(SKUDialog), userInput, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> NewTrackingNumber(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = new UserRequest();
            if (stepContext.Result.Equals("no") || stepContext.Result.Equals("No"))
            {

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for using the GLD Bot") }, cancellationToken);

            }
            else
            {
                userInput.SKUNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(" Invetory Status of Sku# " + userInput.SKUNumber + ":" + "\nItem 1: 22 units\nLocation: Warehouse 6\nThank you for using the GLDBot!") }, cancellationToken);
            }



        }

    }
}
