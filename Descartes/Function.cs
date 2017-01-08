using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using Descartes.DataContracts;
using System.Net.Http;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Descartes
{
    public class Function
    {
        public string FunctionHandler(RequestBody input, ILambdaContext context)
        {
            LambdaLogger.Log(JsonConvert.SerializeObject(input));
    
            foreach(var entry in input.bodyjson.entry)
            {
                var pageID = entry.id;
                var timeOfEvent = entry.time;
    
                foreach(var messageEvent in entry.messaging)
                {
                    if (messageEvent.message != null) 
                    {
                        LambdaLogger.Log(JsonConvert.SerializeObject(messageEvent.message));
                        ReceivedMessage(messageEvent);
                    } 
                    else 
                    {
                        LambdaLogger.Log("Webhook received unknown event: " + JsonConvert.SerializeObject(messageEvent));
                    }
                }
            }   

            Console.WriteLine("End of message handler.");
                    
            return "End.";
        }

        private void ReceivedMessage(Messaging messageEvent)
        {
            var senderID = messageEvent.sender.id;
            var recipientID = messageEvent.recipient.id;
            var timeOfMessage = messageEvent.timestamp;
            var message = messageEvent.message;

            Console.WriteLine("Received message for user {1} from user {0} at {2} with message:", senderID, recipientID, timeOfMessage);
            Console.WriteLine(JsonConvert.SerializeObject(message));

            var messageId = message.mid;
            var messageText = message.text;
            var messageAttachments = message.attachments;

            if (!string.IsNullOrEmpty(messageText)) {

                // If we receive a text message, check to see if it matches a keyword
                // and send back the example. Otherwise, just echo the text we received.
                switch (messageText) {
                case "generic":
                    sendGenericMessage(senderID);
                    break;

                default:
                    sendTextMessage(senderID, messageText);
                    break;
                }
            } else if (!string.IsNullOrEmpty(messageAttachments)) {
                sendTextMessage(senderID, "Message with attachment received");
            }
        }

        private void sendGenericMessage(string recipientId)
        {

        }

        private void sendTextMessage(string recipientId, string messageText)
        {
            var message = new OutboundMessaging
            {
                recipient = new Participant
                {
                    id = recipientId
                },
                message = new OutboundMessage
                {
                    text = messageText
                }
            };

            callSendAPI(message).Wait();
        }

        private static async Task callSendAPI(OutboundMessaging message)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(message), System.Text.Encoding.UTF8, "application/json");

                var url = "https://graph.facebook.com/v2.6/me/messages?access_token=";

                try
                {
                    var response = await client.PostAsync(url, content);

                    Console.WriteLine("Status: " + response.StatusCode);

                    if (response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Message send failed: " + ex.Message);
                }
 
            } 
        }
    }
}