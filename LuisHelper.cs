// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreBot1
{
    public static class LuisHelper
    {
        public static async Task<UserRequest> ExecuteLuisQuery(IConfiguration configuration, ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var userRequest = new UserRequest();

            try
            {
                // Create the LUIS settings from configuration.
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://" + configuration["LuisAPIHostName"]
                );

                
                var recognizer = new LuisRecognizer(luisApplication);
                
                // The actual call to LUIS
                var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);

                var (intent, score) = recognizerResult.GetTopScoringIntent();
                if (intent == "Order")
                {
                    // We need to get the result from the LUIS JSON which at every level returns an array.
                    userRequest.setOrder(true);
                    userRequest.setInventory(false);
                }
                else if (intent == "InventoryDetails")
                {
                    userRequest.setInventory(true);
                }
                else if (intent == "upsOrderNumber")
                {
                    userRequest.isUPSTrackingNumber = true;
                }
                else if (intent == "trackingNumber")
                {
                    userRequest.isTrackingNumber = true;
                }
                else if (intent == "purchaseOrderNumber")
                { 
                    userRequest.ispurchaseOrderNumber = true;

                }else if (intent == "skuNumber")
                {
                    userRequest.isSKUNumber = true;
                }else if(intent == "OrderNumber")
                {
                    userRequest.isOrderNumber = true;
                }
                else if(intent == "Cancel")
                {
                    userRequest.cancel = true;
                }else if(intent == "Yes")
                {
                    userRequest.yes = true;
                }else if(intent == "restart")
                {
                    userRequest.restart = true;
                }else if(intent == "1z")
                {
                    userRequest.is1z = true;
                }else if(intent == "Job")
                {
                    userRequest.isjob = true;
                }
                
            }
            catch (Exception e)
            {
                logger.LogWarning($"LUIS Exception: {e.Message} Check your LUIS configuration.");
            }

            return userRequest;
        }
    }
}

