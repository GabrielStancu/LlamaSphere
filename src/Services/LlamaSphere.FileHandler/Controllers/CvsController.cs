using LlamaSphere.API.DTOs;
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
            var id = await _cvUploadService.UploadFileAsync(file);
            return Ok(id);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception {ex.Message} @ {ex.StackTrace}";
            _logger.LogError(errorMessage, ex);

            return new BadRequestObjectResult(errorMessage);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetCvsForProject(FindDevMatches findDevMatches)
    {
        var reasoningResponse = await _jobMatchingCvsService.GetMatchingCvsForJobAsync(findDevMatches);

        return Ok(reasoningResponse);
    }
}
