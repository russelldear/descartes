using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Descartes
{
    public class GifGetter
    {
        public static async Task<string> New(string text = "")
        {
            using (var client = new HttpClient())
            {
                var url = "http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC";

                if (!String.IsNullOrEmpty(text))
                {
                    var encodedString = System.Uri.EscapeUriString(text);

                    url = String.Format("{0}{1}{2}", url, "&tag=", encodedString);
                }

                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("Giphy request status: " + response.StatusCode);

                    if (response.Content != null)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseString);

                        dynamic responseObject = JObject.Parse(responseString);
                        return responseObject.data.image_url;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Giphy request failed: " + ex.Message);
                }
            }

            return string.Empty;
        }
    }
}