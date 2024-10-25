using LlamaSphere.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CvsController : ControllerBase
{
    private readonly ICvUploadService _cvUploadService;
    private readonly ILogger<CvsController> _logger;

    public CvsController(ICvUploadService cvUploadService,
        ILogger<CvsController> logger)
    {
        _cvUploadService = cvUploadService;
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
}
