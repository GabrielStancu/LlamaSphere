namespace LlamaSphere.API.Services;

public interface ICvUploadService
{
    Task UploadFileAsync(IFormFile file);
}