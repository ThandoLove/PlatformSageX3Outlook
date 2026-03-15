using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IServices
{
    public interface IKnowledgeService
    {
        Task<KnowledgeDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<KnowledgeDto>> SearchAsync(string query);

       
    }

}
