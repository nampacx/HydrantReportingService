using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.WindowsAzure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using HydrantReportingService.Library;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;

namespace HydrantReportingService.Functions
{
    public class RequestSasUri
    {
        public RequestSasUri(IConfiguration configuration, ILogger<RequestSasUri> logger )
        {
            storageAccountConnectionString = configuration.GetValue<string>("AzureWebJobsStorage");
            containerName = configuration.GetValue<string>("CosmosCollection");
            this.logger = logger;
            blobPermissions = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(2));
            containerPermissions = new BlobSasBuilder(BlobContainerSasPermissions.All, DateTimeOffset.UtcNow.AddHours(6));
        }

        private readonly ILogger<RequestSasUri> logger;
        private BlobSasBuilder blobPermissions;
        private readonly BlobSasBuilder containerPermissions;
        private readonly string storageAccountConnectionString;

        public string containerName { get; }

        [FunctionName("RequestSasUri")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reports/{reportId}/images/requestupload/")] HttpRequest req, string reportId, 
            [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from reports")]
            IEnumerable<HydrantReportDTO> reports,
            ILogger log)
        {
            logger.LogInformation($"{nameof(RequestSasUri)} for {reportId} exectued");
            if (reports.Any(r => r.Id == reportId))
            {
                try
                {
                    var blobContainerClient = new BlobContainerClient(storageAccountConnectionString, containerName);
                    blobContainerClient.CreateIfNotExists();
                    var sas = blobContainerClient.GenerateSasUri(containerPermissions);
                    return new OkObjectResult(sas);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            else
            {
                return new StatusCodeResult(400);
            }
        }

        [FunctionName("GetImagesForReport")]
        public async Task<IActionResult> Get(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reports/{reportId}/images/")] HttpRequest req, string reportId,
           [CosmosDB(
                databaseName: "%CosmosDatabase%",
                collectionName: "%CosmosCollection%",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from reports")]
            IEnumerable<HydrantReportDTO> reports,
           ILogger log)
        {
            logger.LogInformation($"{nameof(RequestSasUri)} for {reportId} exectued");
            if (reports.Any(r => r.Id == reportId))
            {
                try
                {
                    var blobContainerClient = new BlobContainerClient(storageAccountConnectionString, containerName);
                    blobContainerClient.CreateIfNotExists();
                    var resultSegment = blobContainerClient.GetBlobsByHierarchyAsync(prefix: reportId).AsPages();
                    var blobs = new List<string>();

                    await foreach (var blobPage in resultSegment)
                    {
                        foreach (var blobhierarchyItem in blobPage.Values)
                        {
                            if (blobhierarchyItem.IsBlob)
                            {
                                var blob = blobContainerClient.GetBlobClient(blobhierarchyItem.Blob.Name);
                                blobs.Add(blob.GenerateSasUri(blobPermissions).AbsoluteUri);
                            }
                        }
                    }

                    return new OkObjectResult(blobs);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            else
            {
                return new StatusCodeResult(400);
            }
        }
    }
}
