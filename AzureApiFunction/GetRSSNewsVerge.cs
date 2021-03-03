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

namespace AzureApiFunction
{
    public static class GetRSSNewsVerge
    {
        [FunctionName("GetRSSNewsVerge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string apiResponse;
            using(var http=new HttpClient())
            {
                using (var response=
                    await http.GetAsync("https://www.theverge.com/rss/index.xml"))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("response");
                    Console.WriteLine(apiResponse);
                }
            }

            return new OkObjectResult(apiResponse);
        }
    }
}
