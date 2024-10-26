using LlamaSphere.API.DTOs;
using LlamaSphere.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobUploadService _jobUploadService;
    private readonly IJobMatchingCvsService _jobMatchingCvsService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobUploadService jobUploadService,
        IJobMatchingCvsService jobMatchingCvsService,
        ILogger<JobsController> logger)
    {
        _jobUploadService = jobUploadService;
        _jobMatchingCvsService = jobMatchingCvsService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<JobUploadedResponse>> UploadFile(IFormFile file)
    {
        try
        {
            var id = await _jobUploadService.UploadFileAsync(file);

            return Ok(new JobUploadedResponse
            {
                Id = id
            });
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception {ex.Message} @ {ex.StackTrace}";
            _logger.LogError(errorMessage, ex);

            return new BadRequestObjectResult(errorMessage);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetJobsForCv(FindJobMatches findJobMatches)
    {
        var cvMatchingJobs = await _jobMatchingCvsService.GetMatchingJobsForCvAsync(findJobMatches);

        return Ok(cvMatchingJobs);
    }
}
