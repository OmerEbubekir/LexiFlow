using LexiFlow.Application.Features.Quiz.Commands;
using LexiFlow.Application.Features.Quiz.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class QuizController : BaseApiController
{
    private readonly IMediator _mediator;

    public QuizController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("today-review")]
    public async Task<IActionResult> GetTodayReview()
    {
        var result = await _mediator.Send(new GetTodayReviewQuery(GetUserId()));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("learned-words")]
    public async Task<IActionResult> GetLearnedWords()
    {
        var result = await _mediator.Send(new GetLearnedWordsQuery(GetUserId()));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("submit-answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        var result = await _mediator.Send(new SubmitAnswerCommand(GetUserId(), request.WordId, request.IsCorrect));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record SubmitAnswerRequest(Guid WordId, bool IsCorrect);
