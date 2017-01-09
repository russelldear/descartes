using System;
using System.IO;
using Descartes.DataContracts;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start.");

            Environment.SetEnvironmentVariable("fb_token", "fake_token");

            TestEchoMessage();
            TestGifMessage();
            TestTrainMessage();
            TestWeatherMessage();

            Console.WriteLine("End.");
        }

        private static void TestEchoMessage()
        {
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
                                    },
                                    ""sender"": {
                                        ""id"": ""sender id""
                                    },
                                    ""recipient"": {
                                        ""id"": ""recipient id""
                                    },
                                    ""timestamp"": ""message timestamp""
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
        }

        private static void TestGifMessage()
        {
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
                                        ""text"": ""gif yoda""
                                    },
                                    ""sender"": {
                                        ""id"": ""sender id""
                                    },
                                    ""recipient"": {
                                        ""id"": ""recipient id""
                                    },
                                    ""timestamp"": ""message timestamp""
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
        }

        private static void TestTrainMessage()
        {
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
                                        ""text"": ""train AVA""
                                    },
                                    ""sender"": {
                                        ""id"": ""sender id""
                                    },
                                    ""recipient"": {
                                        ""id"": ""recipient id""
                                    },
                                    ""timestamp"": ""message timestamp""
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
        }

        private static void TestWeatherMessage()
        {
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
                                        ""text"": ""weather""
                                    },
                                    ""sender"": {
                                        ""id"": ""sender id""
                                    },
                                    ""recipient"": {
                                        ""id"": ""recipient id""
                                    },
                                    ""timestamp"": ""message timestamp""
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
