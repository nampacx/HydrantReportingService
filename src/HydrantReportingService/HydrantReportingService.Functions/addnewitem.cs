using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;

namespace HydrantReportingService.Functions
{
    public static class AddNewItem
    {
        [FunctionName("AddNewItem")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "hrsvc-dev-cosmos-db",
                collectionName: "hrsvc-dev-cosmos-container",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
            ILogger log)
        {
            document = new { Description = req.ReadAsStringAsync().Result, id = Guid.NewGuid() };
            return new OkResult();
        }


    }
}
