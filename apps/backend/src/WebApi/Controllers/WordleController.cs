using LexiFlow.Application.Features.Wordle.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class WordleController : BaseApiController
{
    private readonly IMediator _mediator;

    public WordleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartGame()
    {
        var result = await _mediator.Send(new StartWordleGameCommand(GetUserId()));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{gameId}/guess")]
    public async Task<IActionResult> SubmitGuess(Guid gameId, [FromBody] WordleGuessRequest request)
    {
        var result = await _mediator.Send(new SubmitWordleGuessCommand(GetUserId(), gameId, request.Guess));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record WordleGuessRequest(string Guess);
