using LexiFlow.Application.Features.Story.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class StoryController : BaseApiController
{
    private readonly IMediator _mediator;

    public StoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateStory([FromBody] GenerateStoryRequest request)
    {
        var result = await _mediator.Send(new GenerateStoryCommand(GetUserId(), request.WordIds, request.Language));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record GenerateStoryRequest(List<Guid> WordIds, string Language);
