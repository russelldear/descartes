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

                if (messageText == "generic")
                {
                    sendGenericMessage(senderID);
                }
                else if (messageText.ToLower().StartsWith("gif"))
                {
                    sendGifMessage(senderID, messageText).Wait();
                }
                else if (messageText.ToLower().StartsWith("train"))
                {
                    sendTrainMessage(senderID, messageText).Wait();
                }
                else
                {
                    sendTextMessage(senderID, messageText);
                }
            } 
            else if (!string.IsNullOrEmpty(messageAttachments)) 
            {
                sendTextMessage(senderID, "Message with attachment received");
            }
        }

        private void sendGenericMessage(string recipientId)
        {

        }

        private static async Task sendGifMessage(string recipientId, string messageText)
        {
            string tagString = messageText.Substring(3).Trim();

            string gifUrl = await GifGetter.New(tagString);

            var message = new OutboundMessaging
            {
                recipient = new Participant
                {
                    id = recipientId
                },
                message = new OutboundMessage
                {
                    text = gifUrl
                }
            };

            MessageSender.Send(message).Wait();
        }

        private static async Task sendTrainMessage(string recipientId, string messageText)
        {
            string stop = messageText.Substring(5).Trim();

            string nextDeparture = await TrainGetter.Get(stop);

            var message = new OutboundMessaging
            {
                recipient = new Participant
                {
                    id = recipientId
                },
                message = new OutboundMessage
                {
                    text = nextDeparture
                }
            };

            MessageSender.Send(message).Wait();
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

            MessageSender.Send(message).Wait();
        }
    }
}