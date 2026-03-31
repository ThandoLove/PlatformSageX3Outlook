using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Services;

public sealed class KnowledgeService : IKnowledgeService
{
    private readonly IKnowledgeRepository _repository;
    private readonly ILogger<KnowledgeService> _logger;

    public KnowledgeService(IKnowledgeRepository repository, ILogger<KnowledgeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<KnowledgeDto?> GetByIdAsync(Guid id)
    {
        var article = await _repository.GetByIdAsync(id);
        if (article == null) return null;

        return new KnowledgeDto(
            article.Id,
            article.Title,
            article.Content,
            article.Category,
            article.Summary,
            article.Url);
    }

    public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<KnowledgeDto>();

        var results = await _repository.SearchAsync(query);

        return results.Select(a => new KnowledgeDto(
            a.Id,
            a.Title,
            a.Content,
            a.Category,
            a.Summary,
            a.Url));
    }

    public async Task<List<KnowledgeDto>> GetRecentArticlesAsync()
    {
        var results = await _repository.GetRecentAsync();

        return results.Select(a => new KnowledgeDto(
            a.Id,
            a.Title,
            a.Content,
            a.Category,
            a.Summary,
            a.Url)).ToList();
    }

    // ADDED: Implementation to satisfy the interface
    public async Task SendKnowledgeAsync(object model)
    {
        _logger.LogInformation("Sending knowledge base article...");
        // TODO: Implement the actual sending logic (e.g., Email or Chat)
        await Task.CompletedTask;
    }
}
