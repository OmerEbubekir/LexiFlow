using System.Security.Claims;
using LexiFlow.Application.Features.Auth.Commands;
using MediatR;

namespace LexiFlow.WebApi.Middlewares;

public class JitProvisioningMiddleware
{
    private readonly RequestDelegate _next;

    public JitProvisioningMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirstValue("user_id") ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = context.User.FindFirstValue(ClaimTypes.Email) ?? context.User.FindFirstValue("email");
            var name = context.User.FindFirstValue("name") ?? context.User.FindFirstValue(ClaimTypes.Name) ?? "User";

            if (!string.IsNullOrEmpty(firebaseUid) && !string.IsNullOrEmpty(email))
            {
                // Provision user if they don't exist
                // We're invoking this per request, MediatR command is lightweight and the handler checks if user exists 
                // But normally we'd cache this or set a claim if already provisioned.
                // For demonstration, we simply call it. The handler has `if (user == null)`
                var response = await mediator.Send(new ProvisionUserCommand(firebaseUid, email, name));

                if (response.Success && response.Data != default)
                {
                    // Add our internal Guid User Id to HttpContext Items so Controllers don't have to query DB
                    context.Items["UserId"] = response.Data;
                }
            }
        }

        await _next(context);
    }
}

public static class JitProvisioningMiddlewareExtensions
{
    public static IApplicationBuilder UseJitProvisioning(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JitProvisioningMiddleware>();
    }
}
