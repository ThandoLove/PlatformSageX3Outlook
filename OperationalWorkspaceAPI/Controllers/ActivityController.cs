using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ActivityController : ApiController // Inherit to get Success()
{
    private readonly IActivityService _service;

    // Production Shield: Constructor Injection
    public ActivityController(IActivityService service)
    {
        _service = service;
    }

    [HttpGet("partner/{partnerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPartner(Guid partnerId)
    {
        // Shield: Use the Service logic we built earlier
        var activities = await _service.GetByRelatedEntityAsync(partnerId);
        return Success(activities);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var activity = await _service.GetByIdAsync(id);
        return activity == null ? NotFoundResponse("Activity not found") : Success(activity);
    }
}
