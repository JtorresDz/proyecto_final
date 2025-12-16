using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [HttpGet("claims")]
    public IActionResult Claims()
    {
        return Ok(User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        }));
    }
}
