using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class TrainGetter
    {
        public static async Task<string> Get(string text = "AVA")
        {
            using (var client = new HttpClient())
            {
                var url = "https://www.metlink.org.nz/api/v1/StopDepartures/";

                if (String.IsNullOrEmpty(text))
                {
                    text = "AVA";
                }
                
                var encodedString = System.Uri.EscapeUriString(text).ToUpper();

                url = String.Format("{0}{1}", url, encodedString);
                
                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("Metlink request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);

                        JObject responseObject = JObject.Parse(responseString);

                        var stop = responseObject["Stop"].Value<string>("Name");
                        var destination = string.Empty;
                        var minutes = -1;
                        var seconds = -1;

                        foreach (var service in responseObject["Services"])
                        {
                            if (service.Value<string>("Direction") != "Inbound")
                            {
                                continue;
                            }

                            destination = service.Value<string>("DestinationStopName");

                            minutes = service.Value<int>("DisplayDepartureSeconds") / 60;
                            seconds = service.Value<int>("DisplayDepartureSeconds") % 60;
                            break;
                        }

                        return string.Format("Next departure from {0} heading to {1} in {2} minutes and {3} seconds.", stop, destination, minutes, seconds);
                    }
                    else
                    {
                        var errorResponse = "Your message should be in the format 'train ABCD', where ABCD is the first four letters of your stop.";
                        return errorResponse;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Metlink request failed: " + ex.Message);
                }
            }

            return string.Empty;
        }
    }
}