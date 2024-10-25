using Azure.Storage.Blobs;
using DocumentFormat.OpenXml.Packaging;
using LlamaSphere.FileHandler.Business.TableStorage;
using LlamaSphere.FileHandler.Configuration;
using LlamaSphere.FileHandler.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LlamaSphere.FileHandler.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly ITableStorageClient<JobEntity> _tableStorageClient;
    private readonly BlobStorageConfiguration _blobStorageConfiguration;

    public JobsController([FromKeyedServices("job")] ITableStorageClient<JobEntity> tableStorageClient,
        IOptions<BlobStorageConfiguration> blobStorageConfiguration)
    {
        _tableStorageClient = tableStorageClient;
        _blobStorageConfiguration = blobStorageConfiguration.Value;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var content = await ExtractFileContentAsync(file);

        await UploadFileContentToTableStorageAsync(content);
        await UploadFileToBlobStorageAsync(file);

        return Ok();
    }

    private static async Task<string> ExtractFileContentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded");
        }

        if (Path.GetExtension(file.FileName) != ".docx")
        {
            throw new ArgumentException("Bad file format uploaded. Only accepts '.docx'");
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var wordDoc = WordprocessingDocument.Open(stream, false);
        var body = wordDoc.MainDocumentPart?.Document.Body;
        var content = body?.InnerText ?? string.Empty;

        return content;
    }

    private async Task UploadFileContentToTableStorageAsync(string content)
    {
        var jobEntity = new JobEntity
        {
            PartitionKey = Guid.NewGuid().ToString(),
            RowKey = Guid.NewGuid().ToString(),
            Content = content
        };

        await _tableStorageClient.UpsertEntityAsync(jobEntity);
    }

    private async Task UploadFileToBlobStorageAsync(IFormFile file)
    {
        var blobClient = new BlobContainerClient(_blobStorageConfiguration.ConnectionString, _blobStorageConfiguration.JobsContainerName);

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        await blobClient.UploadBlobAsync(file.FileName, stream);
    }
}
