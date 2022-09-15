using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HydrantReportingService.Functions
{
    public static class GetAllItems
    {
        [FunctionName("GetAllItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
             [CosmosDB(
                databaseName:"hrsvc-dev-cosmos-db",
                collectionName:"hrsvc-dev-cosmos-container",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from c")]
                IEnumerable<ToDoItem> toDoItems,
            ILogger log)
        {
            return new OkObjectResult(toDoItems);
        }
    }

    public class ToDoItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        public string Description { get; set; }
    }
}
