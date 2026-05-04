using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;
using System.Linq;

namespace OperationalWorkspaceAPI.Controllers;

public sealed class TaskController : ApiController
{
    private readonly ITaskService _service;
    public TaskController(ITaskService service) => _service = service;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var request = new GetTasksRequest(id.ToString());
        var response = await _service.GetAsync(request, ct);
        var task = response.Tasks.FirstOrDefault();

        return task == null ? NotFoundResponse("Task not found") : Success(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess) return Failure(result.Message);

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Id },
            Success(result));
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var request = new CompleteTaskRequest(id);
        var result = await _service.CompleteAsync(request, ct);

        return result.IsSuccess ? Success(result) : Failure(result.Message);
    }

    // ✅ NEW DELEGATE ENDPOINT (MATCHING YOUR PATTERN)
    [HttpPost("delegate")]
    public async Task<IActionResult> Delegate([FromBody] DelegateTaskRequest request, CancellationToken ct)
    {
        // This follows your service-based architecture
        var result = await _service.DelegateAsync(request, ct);

        return result.IsSuccess ? Success(result) : Failure(result.Message);
    }
}
