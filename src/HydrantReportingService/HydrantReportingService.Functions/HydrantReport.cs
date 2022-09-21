using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

using HydrantReportingService.Library;
using HydrantReportingService.Services.BingMaps;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        private readonly BingMapClient _bingMapClient;

        public HydrantReport(ILogger<HydrantReport> log, BingMapClient client)
        {
            _logger = log;
            _bingMapClient = client;
        }

        [FunctionName("CreateHydrantReport")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
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
            var address = await _bingMapClient.GetAddress(data.Latitude, data.Longitude);
            data.Address = address?.Address;
            data.Id = Guid.NewGuid().ToString();
            data.Approved = false;
            await report.AddAsync(data);

            return new OkObjectResult(data);
        }

        [FunctionName("GetHydrantReports")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<FeatureCollection>), Description = "The OK response")]
        public async Task<IActionResult> GetHydrantReportsAsGeoJson(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reports/geojson")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from reports")]
            IEnumerable<HydrantReportDTO> reports)
        {
            var features = new List<Feature>();
            foreach (var report in reports.Where(r => r.Longitude != 0 && r.Latitude != 0))
            {
                var coordinates = new Position(report.Latitude, report.Longitude);
                var geom = new Point(coordinates);
                var properties = new Dictionary<string, object>
                {
                    {"type", report.Type },
                    {"address", report.Address },
                    {"approved", report.Approved },
                    {"id", report.Id }
                };
                features.Add(new Feature(geom, properties));
            }

            var fc = new FeatureCollection(features);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            var resultAsJson = JsonConvert.SerializeObject(fc, Formatting.Indented, settings);
            return new OkObjectResult(resultAsJson);
        }

        [FunctionName("ApproveHydrantReport")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted response")]
        public async Task<IActionResult> ApproveReport(
           [HttpTrigger(AuthorizationLevel.Function, "put", Route = "reports/{reportId}/approve")] HttpRequest req, string reportId,
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

