using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    // Define the model here so both the Service and the Form can see it
    public class SendKnowledgeModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class KnowledgeUIService
    {
        private readonly List<string> _articles = new();

        public Task<List<string>> GetArticlesAsync()
        {
            return Task.FromResult(_articles);
        }

        // Renamed/Added to match your Form's requirements
        public Task SendKnowledgeAsync(SendKnowledgeModel model)
        {
            // For now, we just add the message to our list
            _articles.Add($"{model.SearchQuery}: {model.Message}");
            return Task.CompletedTask;
        }

        public Task AddArticleAsync(string article)
        {
            _articles.Add(article);
            return Task.CompletedTask;
        }
    }
}
