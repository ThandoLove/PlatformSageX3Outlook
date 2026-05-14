
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/system-health")]
public class SystemHealthController : ControllerBase
{
    private readonly ISystemHealthService _healthService;

    public SystemHealthController(ISystemHealthService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result =
            await _healthService.GetSystemHealthAsync();

        return Ok(result);
    }
}