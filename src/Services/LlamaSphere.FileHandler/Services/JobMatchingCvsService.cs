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
                StructuredJob = JsonSerializer.Deserialize<ParsedJob>(job.JsonContent).StructuredJob,
                StructuredCv = JsonSerializer.Deserialize<ParsedCv>(matchingCv.JsonContent).StructuredCv
            };

            reasoningRequest.StructuredJob.HrRequirements = new List<HrRequirement>();
            foreach (var hrRequirement in findDevMatches.Keywords)
            {
                reasoningRequest.StructuredJob.HrRequirements.Add(new HrRequirement
                {
                    Skill = hrRequirement.Key,
                    Weight = hrRequirement.Value
                });
            }

            var response = await _httpClient.PostAsJsonAsync("match", reasoningRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var reasonResponse = JsonSerializer.Deserialize<ReasoningResponse>(responseContent);
            
            responses.Add(reasonResponse);
        }

        return responses.OrderByDescending(r => r.Score).ToList();
    }

    public async Task<List<ReasoningResponse>> GetMatchingJobsForCvAsync(FindJobMatches findJobMatches)
    {
        var cv = await _cvTableStorageClient.GetEntityAsync(findJobMatches.CvId, findJobMatches.CvId);
        var matchingJobs = await _jobTableStorageClient.GetEntitiesAsync();

        var responses = new List<ReasoningResponse>();

        foreach (var matchingJob in matchingJobs)
        {
            var reasoningRequest = new ReasoningRequest
            {
                StructuredJob = JsonSerializer.Deserialize<ParsedJob>(matchingJob.JsonContent).StructuredJob,
                StructuredCv = JsonSerializer.Deserialize<ParsedCv>(cv.JsonContent).StructuredCv
            };

            var response = await _httpClient.PostAsJsonAsync("match", reasoningRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var reasonResponse = JsonSerializer.Deserialize<ReasoningResponse>(responseContent);

            responses.Add(reasonResponse);
        }

        return responses.OrderByDescending(r => r.Score).ToList();
    }
}
