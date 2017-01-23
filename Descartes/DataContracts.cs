
namespace Descartes.DataContracts
{
    public class RequestBody
    {
        public Bodyjson bodyjson { get; set; }
        //public Params params { get; set; }
    }

    public class Bodyjson
    {
        //public string object { get; set; }
        public Entry[] entry { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public string time { get; set; }
        public Messaging[] messaging { get; set; }
    }

    public class Messaging
    {
        public Message message { get; set; }
        public Participant sender { get; set; }
        public Participant recipient { get; set; }
        public string timestamp { get; set; }
    }
    
    public class Message
    {
        public string mid { get; set; }
        public string text { get; set; }
        public string attachments { get; set; }
    }

    public class Participant
    {
        public string id { get; set; }
    }

    public class Path
    {
    }

    public class Querystring
    {
    }

    public class Parameters
    {
        public Path path { get; set; }
        public Querystring querystring { get; set; }
    }

    public class OutboundMessaging
    {
        public Participant recipient { get; set; }
        public OutboundMessage message { get; set; }
    }

    public class OutboundMessage
    {
        public string text { get; set; }
        public Attachment attachment { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public string url { get; set; }
    }
}