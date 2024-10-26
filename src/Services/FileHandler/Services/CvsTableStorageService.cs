using System.Text.Json;
using FileHandler.DTOs;
using FileHandler.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FileHandler.Services;

public class CvsTableStorageService : ICvTableStorageService
{
    private readonly ITableStorageClient<CvEntity> _tableStorageClient;

    public CvsTableStorageService([FromKeyedServices("cvs")] ITableStorageClient<CvEntity> tableStorageClient)
    {
        _tableStorageClient = tableStorageClient;
    }

    public async Task UpdateTableStorageContent(ParsedCv parsedCv)
    {
        var oldEntity = await _tableStorageClient.GetEntityAsync(parsedCv.Id, parsedCv.Id);

        await _tableStorageClient.UpsertEntityAsync(new CvEntity
        {
            PartitionKey = parsedCv.Id,
            RowKey = parsedCv.Id,
            Content = oldEntity.Content,
            JsonContent = JsonSerializer.Serialize(parsedCv)
        });
    }
}
