using LlamaSphere.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobUploadService _jobUploadService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobUploadService jobUploadService,
        ILogger<JobsController> logger)
    {
        _jobUploadService = jobUploadService;
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
}
