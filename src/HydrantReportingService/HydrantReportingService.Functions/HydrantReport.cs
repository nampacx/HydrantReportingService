using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

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
using Newtonsoft.Json.Converters;

namespace HydrantReportingService.Functions
{
    public class HydrantReport
    {
        private readonly ILogger<HydrantReport> _logger;

        public HydrantReport(ILogger<HydrantReport> log)
        {
            _logger = log;
        }

        [FunctionName("CreateHydrantReport")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(HydrantReportDTO), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "reports")] HttpRequest req,
             [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection")]  IAsyncCollector<HydrantReportDTO> report)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<HydrantReportDTO>(requestBody);
            data.Id = Guid.NewGuid().ToString();
            data.Approved = false;
            await report.AddAsync(data);

            return new OkObjectResult(data);
        }

        [FunctionName("GetHydrantReports")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<HydrantReportDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetHydrantReports(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reports")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from reports")]  
            IEnumerable<HydrantReportDTO> reports)
        {
            return new OkObjectResult(reports);
        }

        [FunctionName("GetHydrantReportsAsGeoJson")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<HydrantReportDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetHydrantReportsAsGeoJson(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reports/geojson")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from reports")]
            IEnumerable<HydrantReportDTO> reports)
        {
            var result = reports.Select(r =>
            {
                return r.ConvertToGeoJsonFeature();
            });
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            var resultAsJson = JsonConvert.SerializeObject(result,Formatting.Indented, settings);
            return new OkObjectResult(resultAsJson);
        }

        [FunctionName("ApproveHydrantReport")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<HydrantReportDTO>), Description = "The OK response")]
        public async Task<IActionResult> ApproveReport(
           [HttpTrigger(AuthorizationLevel.Function, "put", Route = "reports/{reportId}/approve")] HttpRequest req,string reportId,
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<HydrantReportDTO> report,
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection")]
            IEnumerable<HydrantReportDTO> reports)
        {
            var dto = reports.First(x => x.Id.Equals(reportId));
            dto.Approved = true;
            await report.AddAsync(dto);
            return new AcceptedResult();
        }
    }
}

