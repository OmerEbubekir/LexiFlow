using LexiFlow.Application.Features.Words.Commands;
using LexiFlow.Application.Features.Words.DTOs;
using LexiFlow.Application.Features.Words.Queries;
using LexiFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class WordsController : BaseApiController
{
    private readonly IMediator _mediator;

    public WordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWords(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isLearned = null)
    {
        var query = new GetWordListQuery(GetUserId(), page, pageSize, search, categoryId, isLearned);
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWord(Guid id)
    {
        var result = await _mediator.Send(new GetWordDetailQuery(id, GetUserId()));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddWord([FromBody] AddWordRequest request)
    {
        var command = new AddWordCommand(
            GetUserId(),
            request.EnglishWord,
            request.TurkishTranslation,
            request.PictureUrl,
            request.AudioUrl,
            request.DifficultyLevel,
            request.CategoryId,
            request.Samples ?? new List<AddWordSampleRequest>());

        var result = await _mediator.Send(command);
        return result.Success ? CreatedAtAction(nameof(GetWord), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWord(Guid id, [FromBody] UpdateWordRequest request)
    {
        var command = new UpdateWordCommand(
            id,
            GetUserId(),
            request.EnglishWord,
            request.TurkishTranslation,
            request.PictureUrl,
            request.AudioUrl,
            request.DifficultyLevel,
            request.CategoryId,
            request.Samples ?? new List<AddWordSampleRequest>());

        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWord(Guid id)
    {
        var result = await _mediator.Send(new DeleteWordCommand(id, GetUserId()));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record AddWordRequest(
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    DifficultyLevel DifficultyLevel,
    Guid? CategoryId,
    List<AddWordSampleRequest>? Samples);

public record UpdateWordRequest(
    string EnglishWord,
    string TurkishTranslation,
    string? PictureUrl,
    string? AudioUrl,
    DifficultyLevel DifficultyLevel,
    Guid? CategoryId,
    List<AddWordSampleRequest>? Samples);
