
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace OperationalWorkspaceUI.UIServices.Workspace
    {
    public class KnowledgeUIService
        {
            private readonly List<string> _articles = new();

            public Task<List<string>> GetArticlesAsync()
            {
                return Task.FromResult(_articles);
            }

            public Task AddArticleAsync(string article)
            {
                _articles.Add(article);
                return Task.CompletedTask;
            }
        }
    }