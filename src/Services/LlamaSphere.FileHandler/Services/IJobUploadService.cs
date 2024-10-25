namespace LlamaSphere.API.Services;

public interface IJobUploadService
{
    Task UploadFileAsync(IFormFile file);
}