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
using System.Collections;

namespace AzureApiFunction
{
    public static class GetRSSNewsVerge
    {
        public static int rand = 50;
        public static int count = 2;
        [FunctionName("GetRSSNewsVerge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Random random = new Random();

            List<NewsModel> newsList = new List<NewsModel>();

            await CoinTelegraph(newsList, count, random);
            await BitcoinNews(newsList, count, random);
            await FinanceMagnates(newsList, count, random);
            await Minergate(newsList, count, random);           
            await TheVerge(newsList, count, random);             
            await CryptoNinjas(newsList, count, random);
            await NewsBTC(newsList, count, random);

            newsList.Sort(delegate (NewsModel news1, NewsModel news2)
            {
                return news1.id.CompareTo(news2.id);
            });

            var sz = JsonConvert.SerializeObject(newsList);

            return new OkObjectResult(sz);
        }

        private static async Task TheVerge(List<NewsModel> nl, int count, Random random)
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
                string thumbnail = _fnames[0].InnerText;
                XmlNodeList entry = xmlDoc.GetElementsByTagName("entry");
               
                count = entry.Count < count ? entry.Count : count;
                for (int i = 0; i < count; i++)
                {
                    string title = entry[i].ChildNodes[2].InnerText;
                    string url = entry[i].ChildNodes[5].InnerText;
                    string author = entry[i].ChildNodes[6].InnerText;
                    //string icon = item[i].ChildNodes[10].InnerText;
                    string published = entry[i].ChildNodes[0].InnerText;
                    nl.Add(new NewsModel() { id = random.Next(rand), newssource = "The Verge", source = "verge", thumbnail = thumbnail, headline = title, newsurl = url,
                        published = published,
                        author = author
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task CoinTelegraph(List<NewsModel> nl, int count, Random random)
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
                    string author = item[i].ChildNodes[5].InnerText.Replace("Cointelegraph By ","");
                    //string icon = item[i].ChildNodes[10].InnerText;
                    string published = item[i].ChildNodes[4].InnerText;

                    int st = item[i].InnerText.IndexOf("<img src=") + 10;
                    int en = item[i].InnerText.IndexOf(".jpg")+4;
                    string thumbnail = item[i].InnerText.Substring(st, en-st);
                    nl.Add(new NewsModel() { id = random.Next(rand), newssource = "Coin Telegraph", source="cointelegraph", thumbnail = thumbnail, headline = title, 
                        newsurl = url ,
                        published = published,
                        author =author
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task BitcoinNews(List<NewsModel> nl, int count, Random random)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("https://news.bitcoin.com/feed"))
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
                    string author = item[i].ChildNodes[2].InnerText;
                    string published = item[i].ChildNodes[3].InnerText;
                    int st = item[i].InnerText.IndexOf("https://news.bitcoin.com/wp-content/");
                    int en = item[i].InnerText.IndexOf(".jpg") + 4;
                    string thumbnail = item[i].InnerText.Substring(st, en - st);
                    nl.Add(new NewsModel() { id = random.Next(rand), newssource = "Bitcoin News", source = "bitcoinnews",
                        thumbnail = thumbnail, headline = title, newsurl = url 
                    , published= published, author=author});
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

     
        private static async Task Minergate(List<NewsModel> nl, int count, Random random)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("https://minergate.com/blog/feed"))
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
                    string author = "";
                    string published = item[i].ChildNodes[2].InnerText;
                    int st = item[i].InnerText.IndexOf("https://blog.minergate.com/wp-content/");
                    int en = item[i].InnerText.IndexOf(".png") + 4;
                    string thumbnail = item[i].InnerText.Substring(st, en - st);
                    nl.Add(new NewsModel()
                    {
                        id = random.Next(rand),
                        newssource = "Minergates",
                        source = "minergate",
                        thumbnail = thumbnail,
                        headline = title,
                        newsurl = url
                    ,
                        published = published,
                        author = author
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


       
        private static async Task NewsBTC(List<NewsModel> nl, int count, Random random)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("http://www.newsbtc.com/feed"))
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
                    string author = "";
                    string published = item[i].ChildNodes[2].InnerText;
                    int st = item[i].InnerText.IndexOf("https://www.newsbtc.com/wp-content/");
                    int en = item[i].InnerText.IndexOf(".png") + 4;
                    string thumbnail = item[i].InnerText.Substring(st, en - st);
                    nl.Add(new NewsModel()
                    {
                        id = random.Next(rand),
                        newssource = "NewsBTC",
                        source = "newsbtc",
                        thumbnail = thumbnail,
                        headline = title,
                        newsurl = url
                    ,
                        published = published,
                        author = author
                    });                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static async Task CryptoNinjas(List<NewsModel> nl, int count, Random random)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("http://www.cryptoninjas.net/feed/"))
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
                    string author = "";
                    string published = item[i].ChildNodes[2].InnerText;
                    int st = item[i].InnerText.IndexOf("https://www.cryptoninjas.net/wp-content/");
                    int en = item[i].InnerText.IndexOf(".png") + 4;
                    string thumbnail = item[i].InnerText.Substring(st, en - st);
                    nl.Add(new NewsModel()
                    {
                        id = random.Next(rand),
                        newssource = "Crypto Ninjas",
                        source = "cryptoninjas",
                        thumbnail = thumbnail,
                        headline = title,
                        newsurl = url
                    ,
                        published = published,
                        author = author
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task FinanceMagnates(List<NewsModel> nl, int count, Random random)
        {
            try
            {
                string apiResponse;
                using (var http = new HttpClient())
                {
                    using (var response =
                        await http.GetAsync("http://www.cryptoninjas.net/feed/"))
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
                    string author = "";
                    string published = item[i].ChildNodes[4].InnerText;
                    int st = item[i].InnerText.IndexOf("https://www.cryptoninjas.net/wp-content/");
                    int en = item[i].InnerText.IndexOf(".jpg") + 4;
                    string thumbnail = item[i].InnerText.Substring(st, en - st);
                    nl.Add(new NewsModel()
                    {
                        id = random.Next(rand),
                        newssource = "Finance Magnates",
                        source = "financemagnates",
                        thumbnail = thumbnail,
                        headline = title,
                        newsurl = url
                    ,
                        published = published,
                        author = author
                    });
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
        public string newssource { get; set; }
        public string headline { get; set; }
        public string content { get; set; }
        public string newsurl { get; set; }
        public string thumbnail { get; set; }           
        public string published { get; set; }
        public int hrsago { get; set; }
        public string keywords { get; set; }
        public string author { get; set; }
    }
}
