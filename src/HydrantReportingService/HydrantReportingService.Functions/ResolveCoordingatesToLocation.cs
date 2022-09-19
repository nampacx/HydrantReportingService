using System.IO;
using System.Net;
using System.Threading.Tasks;

using HydrantReportingService.Services.BingMaps;

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
    public class ResolveCoordingatesToLocation
    {
        private readonly ILogger<ResolveCoordingatesToLocation> _logger;
        private readonly BingMapClient _mapClient;

        public ResolveCoordingatesToLocation(BingMapClient client, ILogger<ResolveCoordingatesToLocation> log)
        {
            _logger = log;
            _mapClient = client;
        }

        [FunctionName("ResolveCoordingatesToLocation")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "resolve/coordinates/{latitude}/{longitude}")] HttpRequest req, double latitude, double longitude)
        {
            var location = await _mapClient.GetAddress(latitude, longitude);

            return new OkObjectResult(location);
        }
    }
}

