using LexiFlow.Application.Features.Auth.Commands;
using LexiFlow.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class AuthController : BaseApiController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var result = await _mediator.Send(new GetUserSettingsQuery(GetUserId()));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateUserSettingsRequest request)
    {
        // Use mapped internal user id
        var result = await _mediator.Send(new UpdateUserSettingsCommand(GetUserId(), request.DailyNewWordLimit));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record UpdateUserSettingsRequest(int DailyNewWordLimit);
