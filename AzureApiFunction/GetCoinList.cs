using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections; 
namespace AzureApiFunction
{
    public static class GetCoinList
    {
        public static ArrayList coins;

        [FunctionName("GetCoinList")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {          
            CreateList();
            int rounding = 2;
            try
            {
                log.LogInformation("Coin query started.");
                var API_KEY = "2563a5a6-1274-4943-b0b4-da724381039c";
                var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");     

                var client = new WebClient();
                client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
                client.Headers.Add("Accepts", "application/json");
                string jsonToReturn = client.DownloadString(URL.ToString());
                string jsonFormatted = JValue.Parse(jsonToReturn).ToString(Formatting.Indented);
                Root obj = JsonConvert.DeserializeObject<Root>(jsonFormatted);
                List<CoinList> ls = new List<CoinList>();
                foreach (var s in coins)
                {
                    var val = obj.data.Find(x => x.symbol == s.ToString());
                    if (val != null)
                    { 
                        ls.Add(new CoinList() {
                        source="cmc",
                        id = val.id,
                        name = val.name,
                        symbol = val.symbol,
                        price = Math.Round(val.quote.USD.price, rounding),
                        last_updated = val.last_updated,
                        rank = val.cmc_rank,
                        market_cap = Math.Round(val.quote.USD.market_cap, rounding),
                        volume_24h = Math.Round(val.quote.USD.volume_24h, rounding),
                        percent_change_1h = Math.Round(val.quote.USD.percent_change_1h, rounding),
                        percent_change_24h = Math.Round(val.quote.USD.percent_change_24h, rounding),
                        percent_change_30d = Math.Round(val.quote.USD.percent_change_30d, rounding),
                        percent_change_7d = Math.Round(val.quote.USD.percent_change_7d, rounding)
                        });
                    }
                }
                var sz = JsonConvert.SerializeObject(ls);
                log.LogInformation("Coin query processed successfully.");
                return new OkObjectResult(sz);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }            
        }

        public static void CreateList()
        {
            coins = new ArrayList();
            coins.Add("BTC");
            coins.Add("ETH");
            coins.Add("LTC");
            coins.Add("LINK");
            coins.Add("XLM");
            coins.Add("USDC");
            coins.Add("BCH");
            coins.Add("UNI");
            coins.Add("WBTC");
            coins.Add("AAVE");
            coins.Add("ATOM");
            coins.Add("EOS");
            coins.Add("XTZ");
            coins.Add("DAI");
            coins.Add("SNX");
            coins.Add("ALGO");
            coins.Add("MKT");
            coins.Add("DASH");
            coins.Add("GRT");
            coins.Add("COMP");
            coins.Add("ZEC");
            coins.Add("ETC");
            coins.Add("YFI");
            coins.Add("UMA");
            coins.Add("REN");
            coins.Add("ZRX");
            coins.Add("BAT");
            coins.Add("CGLD");
            coins.Add("BNT");
            coins.Add("LRC");
            coins.Add("OMG");
            coins.Add("MANA");
            coins.Add("KNC");
            coins.Add("NU");
            coins.Add("REP");
            coins.Add("BAND");
            coins.Add("BAL");
            coins.Add("CVC");
            coins.Add("NMR");
            coins.Add("OXT");
            coins.Add("DNT");
        }
    }

    public class CoinList
    {
        public string source { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public double price { get; set; }
        public DateTime last_updated { get; set; }
        public double volume_24h { get; set; }
        public double percent_change_1h { get; set; }
        public double percent_change_24h { get; set; }
        public double percent_change_7d { get; set; }
        public double percent_change_30d { get; set; }
        public double market_cap { get; set; }
        public int rank { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Status
    {
        public DateTime timestamp { get; set; }
        public int error_code { get; set; }
        public object error_message { get; set; }
        public int elapsed { get; set; }
        public int credit_count { get; set; }
        public object notice { get; set; }
        public int total_count { get; set; }
    }

    public class Platform
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public string token_address { get; set; }
    }

    public class USD
    {
        public double price { get; set; }
        public double volume_24h { get; set; }
        public double percent_change_1h { get; set; }
        public double percent_change_24h { get; set; }
        public double percent_change_7d { get; set; }
        public double percent_change_30d { get; set; }
        public double market_cap { get; set; }
        public DateTime last_updated { get; set; }
    }

    public class Quote
    {
        public USD USD { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public int num_market_pairs { get; set; }
        public DateTime date_added { get; set; }
        public List<string> tags { get; set; }
        public long? max_supply { get; set; }
        public double circulating_supply { get; set; }
        public double total_supply { get; set; }
        public Platform platform { get; set; }
        public int cmc_rank { get; set; }
        public DateTime last_updated { get; set; }
        public Quote quote { get; set; }
    }

    public class Root
    {
        public Status status { get; set; }
        public List<Datum> data { get; set; }
    }


}
