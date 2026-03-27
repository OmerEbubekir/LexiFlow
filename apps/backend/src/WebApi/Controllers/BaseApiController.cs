using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetUserId()
    {
        if (HttpContext.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is Guid userId)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User is not provisioned or authenticated properly.");
    }
}
