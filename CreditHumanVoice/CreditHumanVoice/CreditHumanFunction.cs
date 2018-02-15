using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;

namespace CreditHumanVoice
{
    public class CreditHumanFunction
    {
        public SkillResponse CreditHumanFunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;

            var log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = BadInput(resource);
            }

            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;
                log.LogLine($"Intent type: {intentRequest.Intent.Name}");
                log.LogLine($"Intent: {intentRequest}");
                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetPendingTransactionsIntent":
                        log.LogLine($"GetPendingTransactionsIntent sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetPendingTransactions(resource);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetCreditScoreIntent":
                        log.LogLine($"GetCreditScoreIntent sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetCreditScore(resource);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetCurrentBalanceIntent":
                        log.LogLine($"GetCurrentBalanceIntent sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetCurrentBalance(resource);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetRecentActivityIntent":
                        log.LogLine($"GetRecentActivityIntent sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetRecentActivity(resource);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "TransferFundsIntent":
                        log.LogLine($"TransferFundsIntent sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = TransferFunds(resource, intentRequest);
                        response.Response.ShouldEndSession = false;
                        break;
                    default:
                        log.LogLine($"BadInput sent");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = BadInput(resource);
                        response.Response.ShouldEndSession = true;
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        //dummy content, replace with call to get actual data
        private string BadInput(BankingResource resource)
        {
            return "I'm sorry, I didn't understand.  You can say tell me my current balance, tell me my pending transactions, tell me recent activity, tell me my credit score, or, you can say exit... What can I help you with?";
        }

        //dummy content, replace with call to get actual data
        private string GetPendingTransactions(BankingResource resource)
        {
            string response = "As of September 16th you have 2 pending transactions.";
            response += "Debit of 12 dollars and 76 cents at Valero.";
            response += "Deposit of 100 dollars and 0 cents from Your Employer.";
            return response;
        }

        //dummy content, replace with call to get actual data
        private string GetCreditScore(BankingResource resource)
        {
            string response = "As of March 2nd your credit score is 702.";
            return response;
        }

        //dummy content, replace with call to get actual data
        private string GetCurrentBalance(BankingResource resource)
        {
            string response = "As of September 16th your current balance is 1023 dollars and 62 cents.";
            return response;
        }

        //dummy content, replace with call to get actual data
        private string GetRecentActivity(BankingResource resource)
        {
            string t0 = "Withdrawal 40 dollars from ATM at Valero #1424 on September 9th, 2017.";
            string t1 = "Withdrawal 40 dollars from ATM at Valero #1424 on September 12th, 2017.";
            string t2 = "Deposit 800 dollars on September 9th, 2017.";
            string t3 = "Withdrawal 40 dollars from ATM at Valero #4224 on September 16th, 2017.";
            string t4 = "Debit 161 dollars and 18 cents at Walmart #68417 on September 16th, 2017.";
            return $"Your last five transactions are {t0}, {t1}, {t2}, {t3}, {t4}";
        }

        //dummy content, replace with call to get actual data
        private string TransferFunds(BankingResource resource, IntentRequest request)
        {
            string dollars = string.IsNullOrEmpty(request.Intent.Slots["DollarSlot"].Value) ? "" : request.Intent.Slots["DollarSlot"].Value + " dollars ";
            string cents = string.IsNullOrEmpty(request.Intent.Slots["CentSlot"].Value) ? "" : request.Intent.Slots["CentSlot"].Value + " cents ";
            string and = !string.IsNullOrEmpty(request.Intent.Slots["DollarSlot"].Value) && !string.IsNullOrEmpty(request.Intent.Slots["CentSlot"].Value) ? " and " : " ";
            string from = request.Intent.Slots["FromShareSlot"].Value;
            string to = request.Intent.Slots["ToShareSlot"].Value;
            return $"You want me to transfer {dollars}{and}{cents}from {from} to {to}.  Is that correct?";
        }

        public List<BankingResource> GetResources()
        {
            List<BankingResource> resources = new List<BankingResource>();
            BankingResource enUSResource = new BankingResource("en-US");
            enUSResource.SkillName = "Credit Human Voice";
            enUSResource.ResponsePreface = "As of January first 2017: ";
            enUSResource.HelpMessage = "You can say tell me my current balance, tell me my pending transactions, tell me recent activity, tell me my credit score, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me my current balance, tell me my pending transactions, tell me recent activity, tell me my credit score, or, you can say exit.";
            enUSResource.StopMessage = "Goodbye!";
            resources.Add(enUSResource);
            return resources;
        }
    }
}