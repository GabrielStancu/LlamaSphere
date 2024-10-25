using LlamaSphere.API.Business.TableStorage;
using LlamaSphere.API.DTOs;
using LlamaSphere.API.Entities;

namespace LlamaSphere.API.Services;

public class JobMatchingCvsService : IJobMatchingCvsService
{
    private readonly ITableStorageClient<JobEntity> _jobTableStorageClient;
    private readonly ITableStorageClient<CvEntity> _cvTableStorageClient;

    public JobMatchingCvsService([FromKeyedServices("job")] ITableStorageClient<JobEntity> jobTableStorageClient,
        [FromKeyedServices("cv")] ITableStorageClient<CvEntity> cvTableStorageClient)
    {
        _jobTableStorageClient = jobTableStorageClient;
        _cvTableStorageClient = cvTableStorageClient;
    }

    public async Task<JobMatchingCvs> GetMatchingCvsForJobAsync(Guid projectId)
    {
        var job = await _jobTableStorageClient.GetEntityAsync(projectId.ToString(), projectId.ToString());
        var matchingCvs = await _cvTableStorageClient.GetEntitiesAsync();
        var jobMatchingCvs = new JobMatchingCvs
        {
            Job = job.Content,
            Cvs = matchingCvs.Select(cv => cv.Content)
        };

        return jobMatchingCvs;
    }
}
