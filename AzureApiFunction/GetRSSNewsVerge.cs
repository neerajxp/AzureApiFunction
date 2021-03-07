using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace AzureApiFunction
{
    public static class GetRSSNewsVerge
    {
        [FunctionName("GetRSSNewsVerge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            int count = 2;
            List<NewsModel> nl = new List<NewsModel>();
            await CoinTelegraph(nl, count);
            await TheVerge(nl, count);

            var sz = JsonConvert.SerializeObject(nl);

            return new OkObjectResult(sz);
        }

        private static async Task TheVerge(List<NewsModel> nl, int count)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("https://www.theverge.com/rss/index.xml"))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();                        
                    }
                }

                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new System.IO.StringReader(apiResponse));
                XmlNodeList _fnames = xmlDoc.GetElementsByTagName("icon");
                string icon = _fnames[0].InnerText;
                XmlNodeList entry = xmlDoc.GetElementsByTagName("entry");
                count = entry.Count < count ? entry.Count : count;
                for (int i = 0; i < count; i++)
                {
                    string title = entry[i].ChildNodes[2].InnerText;
                    string url = entry[i].ChildNodes[5].InnerText;
                    nl.Add(new NewsModel() { id = i + 1, icon = icon, headline = title, newsurl = url });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task CoinTelegraph(List<NewsModel> nl, int count)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("https://cointelegraph.com/rss"))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }                
                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new System.IO.StringReader(apiResponse));
                XmlNodeList item = xmlDoc.GetElementsByTagName("item");
                count = item.Count < count ? item.Count : count;
                for (int i = 0; i < count; i++)
                {
                    string title = item[i].ChildNodes[0].InnerText;
                    string url = item[i].ChildNodes[1].InnerText;
                    //string icon = item[i].ChildNodes[10].InnerText;

                    int st = item[i].InnerText.IndexOf("<img src=") + 10;
                    int en = item[i].InnerText.IndexOf(".jpg")+4;
                    string icon = item[i].InnerText.Substring(st, en-st);
                    nl.Add(new NewsModel() { id = i + 1, icon = icon, headline = title, newsurl = url });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }

    public class NewsModel
    {        
        public string source { get; set; }
        public int id { get; set; }            
        public string newsource { get; set; }
        public string headline { get; set; }
        public string content { get; set; }
        public string newsurl { get; set; }
        public string icon { get; set; }           
        public DateTime published { get; set; }
        public int hrsago { get; set; }
        public string keywords { get; set; }         
    }
}
