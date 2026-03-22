using OperationalWorkspace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IRepository
{
    public interface IKnowledgeRepository
    {
        Task<Knowledge?> GetByIdAsync(Guid id);
        Task<IEnumerable<Knowledge>> SearchAsync(string query);

        Task<IEnumerable<Knowledge>> GetRecentAsync();
    }
}