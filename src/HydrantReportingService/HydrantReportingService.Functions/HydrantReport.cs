using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using HydrantReportingService.Library;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace HydrantReportingService.Functions
{
    public class HydrantReport
    {
        private readonly ILogger<HydrantReport> _logger;

        public HydrantReport(ILogger<HydrantReport> log)
        {
            _logger = log;
        }

        [FunctionName("HydrantReport")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "reports")] HttpRequest req,
             [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection")]  IAsyncCollector<HydrantReportDTO> report)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<HydrantReportDTO>(requestBody);
            data.ReportId = Guid.NewGuid();
            data.Approved = false;
            await report.AddAsync(data);

            return new OkObjectResult(data);
        }
    }
}

