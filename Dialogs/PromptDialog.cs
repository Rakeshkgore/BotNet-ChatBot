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
    public class PromptDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        public PromptDialog(IConfiguration configuration, ILogger<UserDialog> logger)
            : base(nameof(PromptDialog))
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
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your number below:") }, cancellationToken);
        }

    }
}