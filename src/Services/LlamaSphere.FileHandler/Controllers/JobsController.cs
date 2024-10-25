using System.Text.Json;
using LlamaSphere.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobUploadService _jobUploadService;
    private readonly ICvMatchingJobsService _cvMatchingJobsService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobUploadService jobUploadService,
        ICvMatchingJobsService cvMatchingJobsService,
        ILogger<JobsController> logger)
    {
        _jobUploadService = jobUploadService;
        _cvMatchingJobsService = cvMatchingJobsService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            await _jobUploadService.UploadFileAsync(file);

            return Ok();
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception {ex.Message} @ {ex.StackTrace}";
            _logger.LogError(errorMessage, ex);

            return new BadRequestObjectResult(errorMessage);
        }
    }

    [HttpGet("{cvId:guid}")]
    public async Task<ActionResult> GetCvsForProject(Guid cvId)
    {
        var cvMatchingJobs = await _cvMatchingJobsService.GetMatchingCvsForJobAsync(cvId);
        var jsonRequest = JsonSerializer.Serialize(cvMatchingJobs);

        // call gpt layer with json object, return the result to the frontend

        return Ok(cvMatchingJobs);
    }
}
