using App.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api")]
public class SystemController : ControllerBase
{
    private readonly IClock _clock;

    public SystemController(IClock clock)
    {
        _clock = clock;
    }

    [HttpGet("now")]
    [ProducesResponseType(typeof(ServerTimeResponse), StatusCodes.Status200OK)]
    public IActionResult GetServerTime()
    {
        return Ok(new ServerTimeResponse { UtcNow = _clock.UtcNow });
    }
}

public class ServerTimeResponse
{
    public DateTime UtcNow { get; set; }
}
