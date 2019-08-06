using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace CoreBot1.Dialogs {
    public class UserDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        public UserDialog(IConfiguration configuration, ILogger <UserDialog> logger)
            : base(nameof(UserDialog))
        {
            Configuration = configuration;
            Logger = logger;
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new OrderDialog(configuration,logger));
            AddDialog(new InventoryDialog(configuration, logger));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]{

            IntroStepAsync,
            ChooseStepAsync,
            FinalStepAsync,
        }));
            InitialDialogId = nameof(WaterfallDialog);
            
        }


        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(Configuration["LuisAppId"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file."), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }
            else
            {

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Where is my order?\" or \"Inventory Details\"") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ChooseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Result != null 
                ? 
                await LuisHelper.ExecuteLuisQuery(Configuration, Logger, stepContext.Context, cancellationToken) 
                : 
                new UserRequest();

            if (userInput.isOrder() == true)
            {
               
                return await stepContext.BeginDialogAsync(nameof(OrderDialog), userInput, cancellationToken);
            }
            else if (userInput.isInventory())
            {
                return await stepContext.BeginDialogAsync(nameof(InventoryDialog), userInput, cancellationToken);
            }
            else if (userInput.cancel == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for using the GLDBot") }, cancellationToken);

            }
            else if (userInput.is1z == true)
            {
                var input = stepContext.Result.ToString(); 
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                { Prompt = MessageFactory.Text(" Status of Order# " + input + ":" + "\nShipping Address:\n12380 Morris Road\nAlpharetta GA 30004\nStatus: Delivered by end of day")
                }, cancellationToken);
            }
            else if(userInput.restart == true)
            {
                return await stepContext.BeginDialogAsync(nameof(UserDialog), userInput, cancellationToken);

            }
            else if (userInput.isjob == true)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please visit our jobs website to learn more : https://www.jobs-ups.com") }, cancellationToken);

            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("I'm sorry something went wrong") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
                 return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                { Prompt = MessageFactory.Text("Thank you for using the GLDBot!") },
                cancellationToken);
        }
    }
}
