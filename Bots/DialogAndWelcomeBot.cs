// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;



namespace CoreBot1.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {

        private const string WelcomeText = @"I'm here to help!";

        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {


        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (ActivityTypes.ConversationUpdate != null)
            {
                foreach (var member in membersAdded)
                {
                    // Greet anyone that was not the target (recipient) of this message.
                    // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {

                        string[] paths = { ".", "Cards", "welcomeCard.json" };
                        string fullPath = Path.Combine(paths);
                        var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                        var response = CreateResponse(turnContext.Activity, welcomeCard);

                        //await turnContext.SendActivityAsync(response, cancellationToken);
                    }
                }
            }

        }


        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome the GLDBot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }

        // Create an attachment message response.
        private Activity CreateResponse(IActivity activity, Attachment attachment)
        {
            var response = ((Activity)activity).CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file.
        private Attachment CreateAdaptiveCardAttachment(string filePath)
        {

            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;

            /*
            // combine path for cross platform support
            string[] paths = { ".", "Cards", "welcomeCard.json" };
            string fullPath = Path.Combine(paths);
            var adaptiveCard = File.ReadAllText(fullPath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
            */
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string temp1 = turnContext.Activity.ChannelData.ToString();
                string boolean = "false";
                Logger.LogInformation(temp1.Length.ToString());
                int len = temp1.Length;
                if (len > 70)
                {


                    if (temp1.Substring(70, 4) != null)
                        boolean = temp1.Substring(70, 4);
                    Logger.LogInformation(temp1.Substring(70, 4));
                    if (boolean.Equals("true"))
                        boolean = "True";
                    else { boolean = "True"; }
                    Logger.LogInformation(boolean);
                    bool entry = System.Convert.ToBoolean(boolean);

                    if (entry)
                    {
                        JToken commandToken = JToken.Parse(turnContext.Activity.Value.ToString());

                        string command = commandToken["action"].Value<string>();
                        string commandPrompt = command;



                        if (commandPrompt.Equals("order"))
                        {
                            string[] paths = { ".", "Cards", "orderCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);

                            await turnContext.SendActivityAsync(response, cancellationToken);
                        }
                        else if (command.ToLowerInvariant() == "inventory")
                        {
                            string[] paths = { ".", "Cards", "InventoryCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);
                        }
                        else if (command.ToLowerInvariant() == "somethingelse")
                        {
                            commandPrompt = "somethingelse";
                            await Dialog.Run(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                        }
                        else if (command.ToLowerInvariant() == "ordernumber")
                        {
                            string[] paths = { ".", "Cards", "orderNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);
                        }
                        else if (command.ToLowerInvariant() == "upsordernumber")
                        {
                            string[] paths = { ".", "Cards", "upsOrderNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);

                        }
                        else if (command.ToLowerInvariant() == "trackingnumber")
                        {
                            string[] paths = { ".", "Cards", "trackingNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);

                        }
                        else if (command.ToLowerInvariant() == "trackingnumber")
                        {
                            string[] paths = { ".", "Cards", "trackingNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);

                        }
                        else if (command.ToLowerInvariant() == "skunumber")
                        {
                            string[] paths = { ".", "Cards", "skuNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);

                        }
                        else if (command.ToLowerInvariant() == "ponumber")
                        {
                            string[] paths = { ".", "Cards", "poNumberCard.json" };
                            string fullPath = Path.Combine(paths);
                            var welcomeCard = CreateAdaptiveCardAttachment(fullPath);
                            var response = CreateResponse(turnContext.Activity, welcomeCard);
                            await turnContext.SendActivityAsync(response, cancellationToken);

                        }
                        else
                        {
                            await turnContext.SendActivityAsync($"I'm sorry, I didn't understand that. Please try again", cancellationToken: cancellationToken);
                        }
                    }
                }
                else
                {
                    await Dialog.Run(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }

            // Run the Dialog with the new message Activity.

        }
    }
}


