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
        // FIX: Match the GetTasksRequest(string UserId) record constructor
        var request = new GetTasksRequest(id.ToString());

        var response = await _service.GetAsync(request, ct);

        // Accessing the 'Tasks' property from TaskListResponse
        var task = response.Tasks.FirstOrDefault();

        return task == null ? NotFoundResponse("Task not found") : Success(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        // FIX: CreateAsync returns a TaskResponse class
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess) return Failure(result.Message);

        // FIX: Use 'result.Id' or 'result.TaskId' based on your class properties
        return CreatedAtAction(
            nameof(Get),
            new { id = result.Id },
            Success(result));
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        // FIX: Match the CompleteTaskRequest(Guid TaskId) record constructor
        var request = new CompleteTaskRequest(id);

        var result = await _service.CompleteAsync(request, ct);

        return result.IsSuccess ? Success(result) : Failure(result.Message);
    }
}
