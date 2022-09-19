using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter.Services
{
    public class UploadService
    { 
        public static void UploadFile(Uri blobSasToken, string reportId, string filePath)
        {
            var blobContainerClient = new BlobContainerClient(blobSasToken);

            var fileName = Path.GetFileName(filePath);
            var targetFile = Path.Combine(reportId,fileName);
            var blob = blobContainerClient.GetBlobClient(targetFile);

            blob.Upload(filePath);
        }
    }
}
