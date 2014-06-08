using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq;

namespace NotissimusTest
{
    internal static class Utils
    {
        public static async Task<yml_catalog> Get(string url)
        {
            Stream xmlStream;
            using (HttpClient client = new HttpClient())
            {
                Task<Stream> xml = client.GetStreamAsync(url);
                xmlStream = await xml;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(yml_catalog));
            return (yml_catalog) xmlSerializer.Deserialize(xmlStream);
        }

        public static async Task<string> Post(yml_catalog model, string id, Uri uri)
        {
            offer offer = model.shop.offers.First(x => x.id == id);

            string json = JsonConvert.SerializeObject(offer,
                Formatting.Indented,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            string response;

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(json);
                HttpResponseMessage responseMessage = await client.PostAsync(uri, content);
                response = await responseMessage.Content.ReadAsStringAsync();
            }

            return response;
        }
    }
}
