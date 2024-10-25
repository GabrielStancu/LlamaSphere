using Azure.Storage.Blobs;
using DocumentFormat.OpenXml.Packaging;
using LlamaSphere.API.Business.TableStorage;
using LlamaSphere.API.Configuration;
using LlamaSphere.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CvsController : ControllerBase
{
    private readonly ITableStorageClient<CvEntity> _tableStorageClient;
    private readonly BlobStorageConfiguration _blobStorageConfiguration;

    public CvsController([FromKeyedServices("cv")] ITableStorageClient<CvEntity> tableStorageClient,
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
        var cvEntity = new CvEntity
        {
            PartitionKey = Guid.NewGuid().ToString(),
            RowKey = Guid.NewGuid().ToString(),
            Content = content
        };

        await _tableStorageClient.UpsertEntityAsync(cvEntity);
    }

    private async Task UploadFileToBlobStorageAsync(IFormFile file)
    {
        var blobClient = new BlobContainerClient(_blobStorageConfiguration.ConnectionString, _blobStorageConfiguration.CvsContainerName);

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        await blobClient.UploadBlobAsync(file.FileName, stream);
    }
}
