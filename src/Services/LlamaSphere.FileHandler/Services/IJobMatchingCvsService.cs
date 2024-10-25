using LlamaSphere.API.DTOs;

namespace LlamaSphere.API.Services;

public interface IJobMatchingCvsService
{
    Task<JobMatchingCvs> GetMatchingCvsForJobAsync(Guid projectId);
}