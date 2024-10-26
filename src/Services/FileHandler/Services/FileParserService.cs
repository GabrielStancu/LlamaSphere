using FileHandler.DTOs;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FileHandler.Services;

public class FileParserService : IFileParserService
{
    private readonly HttpClient _client;

    public FileParserService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("fileParserClient");
    }

    public async Task<ParsedCv> ParseCvFileAsync(string content)
    {
        var result = await _client.PostAsync("cvs", new StringContent(content));
        var resultContent = await result.Content.ReadAsStringAsync();
        var parsedCv = JsonSerializer.Deserialize<ParsedCv>(resultContent);
        
        return parsedCv;
    }

    public async Task<ParsedJob> ParseJobFileAsync(string content)
    {
        var result = await _client.PostAsync("jobs", new StringContent(content));
        var resultContent = await result.Content.ReadAsStringAsync();
        var parsedJob = JsonSerializer.Deserialize<ParsedJob>(resultContent);

        return parsedJob;
    }
}
