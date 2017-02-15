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
                var url = "http://www.metservice.com/publicData/localForecastWellington";

                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("MetService request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);

                        JObject responseObject = JObject.Parse(responseString);

                        var i = 0;
                        JToken today = null;
                        JToken tomorrow = null;

                        foreach (var day in responseObject["days"])
                        {
                            switch(i)
                            {
                                case 0:
                                    today = day;
                                    break;
                                case 1:
                                    tomorrow = day;
                                    break;
                                default:
                                    break;
                            }

                            i++;
                        }

                        var todayForecast = today.Value<string>("forecast");
                        var todayMax = today.Value<string>("max");
                        var todayMin = today.Value<string>("min");

                        var tomorrowForecast = tomorrow.Value<string>("forecast");
                        var tomorrowMax = tomorrow.Value<string>("max");
                        var tomorrowMin = tomorrow.Value<string>("min");
                        
                        result.Add(string.Format("Today: {0} Max {1}°C - Min {2}°C. \r\nTomorrow: {3} Max {4}°C - Min {5}°C.", 
                                                todayForecast, todayMax, todayMin,
                                                tomorrowForecast, tomorrowMax, tomorrowMin));
                    }
                    else
                    {
                        var errorResponse = "Four seasons in one day, as usual.";
                        result.Add(errorResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MetService request failed: " + ex.Message);
                }
            }

            result.Add(string.Empty);

            return result;
        }
    }
}