
using Microsoft.Extensions.Logging;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Interfaces.IRepository;

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
            article.Category);
    }

    public async Task<IEnumerable<KnowledgeDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<KnowledgeDto>();

        _logger.LogInformation("Knowledge search performed for: {Query}", query);

        var results = await _repository.SearchAsync(query);

        return results.Select(a => new KnowledgeDto(
            a.Id,
            a.Title,
            a.Content,
            a.Category));
    }
}
