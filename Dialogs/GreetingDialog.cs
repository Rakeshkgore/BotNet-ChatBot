using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace CoreBot1.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        public GreetingDialog(IConfiguration configuration, ILogger<UserDialog> logger)
            : base(nameof(GreetingDialog))
        {
            Configuration = configuration;
            Logger = logger;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]{
            IntroStepAsync,
        }));
            InitialDialogId = nameof(WaterfallDialog);

        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Where is my order?\" or \"inventory details\"") }, cancellationToken);
        }

    }
}