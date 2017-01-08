using System;
using System.IO;
using Descartes;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start.");

            var testJson = @"
            { ""bodyjson"": { 
                    ""object"": ""page"", 
                    ""entry"": [
                        {
                            ""id"": ""this is an id"",
                            ""time"": ""this is a time"",
                            ""messaging"":
                            [
                                {
                                    ""message"":
                                    {
                                        ""mid"": ""message id"",
                                        ""text"": ""this is text""
                                    }
                                }
                            ]
                        }
                    ]
                }, 
                ""params"": { 
                    ""path"": {}, 
                    ""querystring"": {} 
                } 
            }";

            using (Stream s = GenerateStreamFromString(testJson))
            {
                var requestBody = new Amazon.Lambda.Serialization.Json.JsonSerializer().Deserialize<RequestBody>(s);

                Console.WriteLine(new Descartes.Function().FunctionHandler(requestBody, null));
            }

            Console.WriteLine("End.");
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
