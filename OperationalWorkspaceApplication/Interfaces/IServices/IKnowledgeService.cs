using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IKnowledgeService
    {
        Task<KnowledgeDto?> GetByIdAsync(Guid id);
        Task SendKnowledgeAsync(object model);     // Add this
        Task<IEnumerable<KnowledgeDto>> SearchAsync(string query);


        public async Task<List<KnowledgeDto>> GetRecentArticlesAsync()
        {
            return await Task.FromResult(new List<KnowledgeDto>());
        }

    }

}
