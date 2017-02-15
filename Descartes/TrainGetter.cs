using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class TrainGetter
    {
        private const string departureFormat = "Next departure from {0} heading to {1} leaves in {2} minutes and {3} seconds.\n";

        public static async Task<string> Get(string text = "AVA")
        {
            using (var client = new HttpClient())
            {
                var url = GetUrl(text);

                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("Metlink request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var metlinkResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(metlinkResponse);

                        JObject responseObject = JObject.Parse(metlinkResponse);

                        if (url.EndsWith("WELL"))
                        {
                            return GetWellingtonResponse(responseObject);
                        }
                        else
                        {
                            return GetExternalStationResponse(responseObject);
                        }
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

        private static string GetUrl(string text)
        {
            var url = "https://www.metlink.org.nz/api/v1/StopDepartures/";

            if (String.IsNullOrEmpty(text))
            {
                text = "AVA";
            }

            if (text.Length > 4)
            {
                text = text.Substring(0, 4);
            }

            var encodedString = System.Uri.EscapeUriString(text).ToUpper();

            return String.Format("{0}{1}", url, encodedString);
        }

        private static Dictionary<string, string> GetDestinations()
        {
            return new Dictionary<string, string>()
            {
                {"UPPE", "Upper Hutt"},
                {"TAIT2", "Taita"},
                {"WAIK", "Waikanae"},
                {"PORI2", "Porirua"},
                {"JOHN", "Johnsonville"},
                {"MELL", "Melling"}
            };
        }

        private static string GetWellingtonResponse(JObject responseObject)
        {
            var stop = responseObject["Stop"].Value<string>("Name");

            var responseString = string.Empty;

            var destinations = GetDestinations();

            foreach(var key in destinations.Keys)
            {
                var departure = GetWellingtonDeparture(key, destinations[key], responseObject["Services"]);

                if (departure != null)
                {
                    responseString += string.Format(departureFormat, stop, departure.Destination, departure.Minutes, departure.Seconds);
                }
                else
                {
                    responseString += string.Format("No {0} departures listed.\n", destinations[key]);
                }
            }

            return responseString;
        }

        private static Departure GetWellingtonDeparture(string destinationKey, string destinationName, IEnumerable<JToken> services)
        {
            foreach (var service in services)
            {
                if (service.Value<string>("DestinationStopID") != destinationKey)
                {
                    continue;
                }

                return new Departure
                {
                    Direction = "Outbound",
                    Destination = destinationName,
                    Minutes = service.Value<int>("DisplayDepartureSeconds") / 60,
                    Seconds = service.Value<int>("DisplayDepartureSeconds") % 60
                };
            }

            return null;
        }

        private static string GetExternalStationResponse(JObject responseObject)
        {
            var stop = responseObject["Stop"].Value<string>("Name");
            var inbound = GetExternalDeparture("Inbound", responseObject["Services"]);
            var outbound = GetExternalDeparture("Outbound", responseObject["Services"]);

            var responseString = string.Format(departureFormat, stop, inbound.Destination, inbound.Minutes, inbound.Seconds);
            responseString += string.Format(departureFormat, stop, outbound.Destination, outbound.Minutes, outbound.Seconds);

            return responseString;
        }

        private static Departure GetExternalDeparture(string direction, IEnumerable<JToken> services)
        {
            foreach (var service in services)
            {
                if (service.Value<string>("Direction") != direction || service.Value<string>("ServiceID") == "WRL")
                {
                    continue;
                }

                return new Departure
                {
                    Direction = direction,
                    Destination = service.Value<string>("DestinationStopName"),
                    Minutes = service.Value<int>("DisplayDepartureSeconds") / 60,
                    Seconds = service.Value<int>("DisplayDepartureSeconds") % 60
                };
            }

            return null;
        }
    }

    public class Departure
    {
        public string Direction { get; set; }
        public string Destination { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}