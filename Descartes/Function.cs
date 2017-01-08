using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Descartes
{
    public class Function
    {
        public string FunctionHandler(RequestBody input, ILambdaContext context)
        {
            LambdaLogger.Log(JsonConvert.SerializeObject(input));
    
            if (/*input.bodyjson["object"] == "page"*/ true)
            {
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
                Console.WriteLine("Yup.");
            }
            else 
            {
                Console.WriteLine("Nup.");
            }
                    
            return "End.";
        }

        private void ReceivedMessage(dynamic messageEvent)
        {

        }
    }

    public class Message
    {
        public string mid { get; set; }
        public string text { get; set; }
    }

    public class Messaging
    {
        public Message message { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public string time { get; set; }
        public Messaging[] messaging { get; set; }
    }

    public class Bodyjson
    {
        //public string object { get; set; }
        public Entry[] entry { get; set; }
    }

    public class Path
    {
    }

    public class Querystring
    {
    }

    public class Params
    {
        public Path path { get; set; }
        public Querystring querystring { get; set; }
    }

    public class RequestBody
    {
        public Bodyjson bodyjson { get; set; }
        //public Params params { get; set; }
    }
}