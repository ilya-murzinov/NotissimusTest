using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace NotissimusTest
{
    internal static class Utils
    {
        public static string Json;

        /// <summary>
        ///     Download XML from specified URL and deserialize it to model
        /// </summary>
        /// <param name="url">XML URL</param>
        /// <returns></returns>
        public static async Task<yml_catalog> Get(string url)
        {
            Stream xmlStream;
            using (var client = new HttpClient())
            {
                Task<Stream> xml = client.GetStreamAsync(url);
                xmlStream = await xml;
            }
            var xmlSerializer = new XmlSerializer(typeof (yml_catalog));
            return (yml_catalog) xmlSerializer.Deserialize(xmlStream);
        }

        /// <summary>
        ///     Find offer with specified Id, serialize it to JSON and POST request JSON to specified URI
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">Offer Id</param>
        /// <param name="uri">URI to POST request</param>
        /// <returns></returns>
        public static async Task<string> Post(yml_catalog model, string id, Uri uri)
        {
            offer offer = model.shop.offers.First(x => x.id == id);

            Json = JsonConvert.SerializeObject(offer,
                Formatting.Indented,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            string response;

            using (var client = new HttpClient())
            {
                HttpContent content = new StringContent(Json);
                HttpResponseMessage responseMessage = await client.PostAsync(uri, content);
                response = await responseMessage.Content.ReadAsStringAsync();
            }

            return response;
        }
    }
}