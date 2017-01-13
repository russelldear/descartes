using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class BusGetter
    {
        public static async Task<string> Get(string text = "5008")
        {
            using (var client = new HttpClient())
            {
                var url = "https://www.metlink.org.nz/api/v1/StopDepartures/";

                if (String.IsNullOrEmpty(text))
                {
                    text = "5008";
                }

                url = String.Format("{0}{1}", url, text);
                
                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("Metlink bus request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);

                        JObject responseObject = JObject.Parse(responseString);

                        var stop = responseObject["Stop"].Value<string>("Name");
                        var destination = string.Empty;
                        var route = string.Empty;
                        var minutes = -1;
                        var seconds = -1;

                        var responseText = new StringBuilder(string.Format("Next five buses from {0}: \n\r", stop));

                        var i = 0;

                        foreach(var service in responseObject["Services"])
                        {
                            if (i >= 5)
                            {
                                break;
                            }

                            destination = service.Value<string>("DestinationStopName");
                            route =  service.Value<string>("ServiceID");
                            minutes = service.Value<int>("DisplayDepartureSeconds") / 60;
                            seconds = service.Value<int>("DisplayDepartureSeconds") % 60;

                            responseText.Append(string.Format("Route {0} to {1}: {2} mins {3} seconds\n\r", route, destination, minutes, seconds));

                            i++;
                        }

                        return responseText.ToString();
                    }
                    else
                    {
                        var errorResponse = "Your message should be in the format 'bus 1234', where 1234 is the number of your stop.";
                        return errorResponse;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Metlink bus request failed: " + ex.Message);
                }
            }

            return string.Empty;
        }
    }
}