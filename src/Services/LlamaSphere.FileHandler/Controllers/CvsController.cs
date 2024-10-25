using System.Text.Json;
using LlamaSphere.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CvsController : ControllerBase
{
    private readonly ICvUploadService _cvUploadService;
    private readonly IJobMatchingCvsService _jobMatchingCvsService;
    private readonly ILogger<CvsController> _logger;

    public CvsController(ICvUploadService cvUploadService,
        IJobMatchingCvsService jobMatchingCvsService,
        ILogger<CvsController> logger)
    {
        _cvUploadService = cvUploadService;
        _jobMatchingCvsService = jobMatchingCvsService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            await _cvUploadService.UploadFileAsync(file);
            return Ok();
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception {ex.Message} @ {ex.StackTrace}";
            _logger.LogError(errorMessage, ex);

            return new BadRequestObjectResult(errorMessage);
        }
    }

    [HttpGet("{jobId:guid}")]
    public async Task<ActionResult> GetCvsForProject(Guid jobId)
    {
        var jobMatchingCvs = await _jobMatchingCvsService.GetMatchingCvsForJobAsync(jobId);
        var jsonRequest = JsonSerializer.Serialize(jobMatchingCvs);

        // call gpt layer with json object, return the result to the frontend

        return Ok(jobMatchingCvs);
    }
}
