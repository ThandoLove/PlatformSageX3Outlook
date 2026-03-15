using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.Interfaces.IServices; // Ensure this is imported
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class KnowledgeBaseController : ApiController
{
    // FIX: Renamed from IKnowledgeBaseService to IKnowledgeService
    private readonly IKnowledgeService _service;

    public KnowledgeBaseController(IKnowledgeService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "query" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async System.Threading.Tasks.Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Failure("Search query is required");

        var results = await _service.SearchAsync(query);
        return Success(results);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async System.Threading.Tasks.Task<IActionResult> Get(Guid id)
    {
        var article = await _service.GetByIdAsync(id);
        return article == null ? NotFoundResponse("Article not found") : Success(article);
    }
}
