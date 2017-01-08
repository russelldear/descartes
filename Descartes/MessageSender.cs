using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using Descartes.DataContracts;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class MessageSender
    {
        public static async Task Send(OutboundMessaging message)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(message), System.Text.Encoding.UTF8, "application/json");

                var accessToken = Environment.GetEnvironmentVariable("fb_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("You need to set the access token for the Messenger API in the environment variables.");
                }

                var url = "https://graph.facebook.com/v2.6/me/messages?access_token=" + accessToken;

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