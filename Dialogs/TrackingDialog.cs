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
    public class TrackingDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;


        public TrackingDialog(IConfiguration configuration, ILogger logger)
            : base(nameof(TrackingDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetTrackingNumberAsync,
                ProcessTrackingNumberAsync,
                NewTrackingNumber,
                
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> GetTrackingNumberAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your Tracking Number below:") }, cancellationToken);


        }
        private async Task<DialogTurnResult> ProcessTrackingNumberAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
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
                userInput.trackingNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(" Status of Order# " + userInput.trackingNumber + ":" + "\nShipping Address:\n12380 Morris Road\nAlpharetta GA 30004\nStatus: Delivered by end of day \nIf you would like to check on the status of another order please enter in the number? If not say \"no\"") }, cancellationToken);

            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(TrackingDialog), userInput, cancellationToken);
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
                userInput.trackingNumber = stepContext.Result.ToString();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(" Status of Order# " + userInput.trackingNumber + ":" + "\nShipping Address:\n55 Glenlake Pkwy NE\n Atlanta, GA 30328\nStatus: Delivered by end of day \nThank you for using the GLDBot!") }, cancellationToken);
            }



        }


    }

}


