using Azure.Data.Tables;

namespace LlamaSphere.API.Business.TableStorage;

public interface ITableStorageClient<T> where T : class, ITableEntity
{
    Task<T> GetEntityAsync(string partitionKey, string rowKey);
    Task UpsertEntityAsync(T entity);
}