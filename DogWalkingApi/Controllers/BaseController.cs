using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogWalkingApi.Controllers;

public abstract class BaseController : ControllerBase
{
    protected int? GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return null;
        if (!int.TryParse(userIdStr, out var userId)) return null;
        return userId;
    }
}
