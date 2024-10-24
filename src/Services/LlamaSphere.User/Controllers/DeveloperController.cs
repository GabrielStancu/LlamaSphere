using LlamaSphere.User.DTOs.CreateDeveloper;
using LlamaSphere.User.DTOs.DeleteDeveloper;
using LlamaSphere.User.DTOs.GetDevelopers;
using LlamaSphere.User.DTOs.UpdateDeveloper;
using LlamaSphere.User.Features.Developers;
using LlamaSphere.User.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LlamaSphere.User.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeveloperController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeveloperController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Developer>> GetDeveloperById(Guid id)
    {
        var result = await _mediator.Send(new GetDeveloperByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Developer>>> GetDevelopers([FromQuery] GetDevelopersRequest request)
    {
        var command = request.Adapt<GetDevelopersQuery>();
        var result = await _mediator.Send(command);
        var response = result.Adapt<GetDevelopersResponse>();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<Developer>> CreateDeveloper([FromBody] CreateDeveloperRequest request)
    {
        var command = request.Adapt<CreateDeveloperCommand>();
        var result = await _mediator.Send(command);
        var response = result.Adapt<CreateDeveloperResponse>();

        return Created($"/developers/{response.Id}", response);
    }

    [HttpPost]
    public async Task<ActionResult<Developer>> UpdateDeveloper([FromBody] UpdateDeveloperRequest request)
    {
        var command = request.Adapt<UpdateDeveloperCommand>();
        var result = await _mediator.Send(command);
        var response = result.Adapt<UpdateDeveloperResponse>();

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Developer>> DeleteDeveloper(Guid id)
    {
        var command = new DeleteDeveloperCommand(id);
        var result = await _mediator.Send(command);
        var response = result.Adapt<DeleteDeveloperResponse>();

        return Ok(response);
    }
}
