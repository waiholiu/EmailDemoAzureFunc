using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Specialized;

namespace EmailFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                var formData = req.ReadFormAsync().Result;

                var ms = new MemoryStream();
                formData.Files[0].CopyTo(ms);
                var fileBytes = ms.ToArray();

                var sendingAddress = formData["sendingAddress"].ToString();


                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                                "<INSERT_API_KEY>");
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "mail.testdomain1.cyou", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "Wai Liu Test Email <mailgun@mail.testdomain1.cyou>");
                request.AddParameter("to", sendingAddress);
                request.AddParameter("subject", "Email sender");
                request.AddParameter("text", "You requested this file be sent to you. Here it is.");
                request.AddFile("attachment", fileBytes, formData.Files[0].FileName) ;
                request.Method = Method.POST;
                await client.ExecuteAsync(request);

                return (ActionResult)new OkObjectResult("done");
            }
            catch(Exception ex)
            {
                return (ActionResult)new StatusCodeResult(500);
            }
        }
    }
  
}
