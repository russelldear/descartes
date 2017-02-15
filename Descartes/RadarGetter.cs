using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class RadarGetter
    {
        public static async Task<List<string>> Get(string text = "")
        {
            var result = new List<string>();

            using (var client = new HttpClient())
            {
                var baseUrl = "http://mobile-apps.metservice.com";

                var url = baseUrl + "/publicData/mobileRainRadar_town_Wellington";

                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("MetService radar request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);

                        JObject responseObject = JObject.Parse(responseString);

                        foreach (var imageItem in responseObject["imageList"])
                        {
                            var relativePath = imageItem.Value<string>("url");
                            result.Add(baseUrl + relativePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MetService request failed: " + ex.Message);
                }
            }

            return result;
        }
    }
}