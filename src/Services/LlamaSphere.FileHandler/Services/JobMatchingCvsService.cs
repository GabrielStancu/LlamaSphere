using System.Text.Json;
using LlamaSphere.API.Business.TableStorage;
using LlamaSphere.API.DTOs;
using LlamaSphere.API.Entities;

namespace LlamaSphere.API.Services;

public class JobMatchingCvsService : IJobMatchingCvsService
{
    private readonly ITableStorageClient<JobEntity> _jobTableStorageClient;
    private readonly ITableStorageClient<CvEntity> _cvTableStorageClient;
    private readonly HttpClient _httpClient;

    public JobMatchingCvsService([FromKeyedServices("job")] ITableStorageClient<JobEntity> jobTableStorageClient,
        [FromKeyedServices("cv")] ITableStorageClient<CvEntity> cvTableStorageClient,
        IHttpClientFactory factory)
    {
        _jobTableStorageClient = jobTableStorageClient;
        _cvTableStorageClient = cvTableStorageClient;
        _httpClient = factory.CreateClient("reasoning");
    }

    public async Task<List<ReasoningResponse>> GetMatchingCvsForJobAsync(FindDevMatches findDevMatches)
    {
        var job = await _jobTableStorageClient.GetEntityAsync(findDevMatches.ProjectId, findDevMatches.ProjectId);
        var matchingCvs = await _cvTableStorageClient.GetEntitiesByKeywords(findDevMatches.Keywords);
        var responses = new List<ReasoningResponse>();

        foreach (var matchingCv in matchingCvs)
        {
            var reasoningRequest = new ReasoningRequest
            {
                StructuredCv = JsonSerializer.Deserialize<StructuredCv>(job.JsonContent),
                StructuredJob = JsonSerializer.Deserialize<StructuredJob>(matchingCv.JsonContent),
                Weights = findDevMatches.Keywords
            };

            var response = await _httpClient.PostAsJsonAsync("match", reasoningRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var reasonResponse = JsonSerializer.Deserialize<ReasoningResponse>(responseContent);
            
            responses.Add(reasonResponse);
        }

        return responses.OrderByDescending(r => r.Score).ToList();
    }
}
