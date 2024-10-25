using Azure.Data.Tables;
using LlamaSphere.FileHandler.Configuration;
using LlamaSphere.FileHandler.Entities;
using Microsoft.Extensions.Options;

namespace LlamaSphere.FileHandler.Business.TableStorage;

public class JobsTableStorageClient : TableStorageClient<JobEntity>
{
    private readonly TableStorageConfiguration _configuration;

    public JobsTableStorageClient(IOptions<TableStorageConfiguration> configuration, ILogger<JobsTableStorageClient> logger) : base(logger)
    {
        _configuration = configuration.Value;
    }

    protected override async Task InitTableClientAsync()
    {
        if (TableClient != null)
            return;

        var serviceClient = new TableServiceClient(_configuration.ConnectionString);
        TableClient = serviceClient.GetTableClient(_configuration.JobsTableName);
        await TableClient.CreateIfNotExistsAsync();
    }
}
