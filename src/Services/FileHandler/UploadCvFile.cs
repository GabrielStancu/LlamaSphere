using Azure.Storage.Blobs;
using FileHandler.Configuration;
using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileHandler
{
    public class UploadCvFile
    {
        //private readonly StorageAccountConfiguration _saConfiguration;
        private readonly ILogger<UploadCvFile> _logger;

        public UploadCvFile(//StorageAccountConfiguration saConfiguration,
            ILogger<UploadCvFile> logger)
        {
            //_saConfiguration = saConfiguration;
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var parsedFormBody = MultipartFormDataParser.ParseAsync(req.Body);
            var file = parsedFormBody.Result.Files[0];

            _logger.LogInformation("Received file {ReceivedFile}", file.FileName);

            //var blobClient = new BlobContainerClient(_saConfiguration.ConnectionString, _saConfiguration.ContainerName);
            //await blobClient.UploadBlobAsync(file.FileName, file.Data);

            _logger.LogInformation("Uploaded file {ReceivedFile} to blob storage", file.FileName);

            return new OkResult();
        }
    }
}
