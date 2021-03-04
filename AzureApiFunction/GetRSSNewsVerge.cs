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

namespace AzureApiFunction
{
    public static class GetRSSNewsVerge
    {
        [FunctionName("GetRSSNewsVerge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
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
                        //Console.WriteLine("response");
                        //Console.WriteLine(apiResponse);
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new System.IO.StringReader(apiResponse));
                XmlNodeList _fnames = xmlDoc.GetElementsByTagName("icon");
                string icon = _fnames[0].InnerText;
                XmlNodeList entry = xmlDoc.GetElementsByTagName("entry");
                List<NewsModel> nl = new List<NewsModel>();
                
                for (int i=0;i<entry.Count;i++)
                {
                    string title = entry[i].ChildNodes[2].InnerText;
                    string url =  entry[i].ChildNodes[5].InnerText;
                    nl.Add(new NewsModel() { id=i+1, icon=icon, headline=title, newsurl=url });    
                }

                var sz = JsonConvert.SerializeObject(nl);

                return new OkObjectResult(sz);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
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
