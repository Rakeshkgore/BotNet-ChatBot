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
    public class poNumberDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public poNumberDialog(IConfiguration configuration, ILogger logger)
            : base(nameof(poNumberDialog))
        {
            Configuration = configuration;
            Logger = logger;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetInventoryNumberAsync,
                ProcessInventoryNumberAsync,
                NewTrackingNumber

            }));

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
                userInput.purchaseOrderNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(" Invetory status of Po# " + userInput.purchaseOrderNumber + ":" + "\nItem 1: 65 units\nItem 2: 12 units\nLocation: Warehouse 1\n If you would like to check inventory on another Po# enter in the number? If not say \"no\"") }, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(poNumberDialog), userInput, cancellationToken);
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
                userInput.purchaseOrderNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(" Invetory Status of Po# " + userInput.purchaseOrderNumber + ":" + "\nItem 1: 22 units\nItem 2: 367 units\nLocation: Warehouse 6\nThank you for using the GLDBot!") }, cancellationToken);
            }



        }


    }
}
