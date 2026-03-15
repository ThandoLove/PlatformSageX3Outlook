using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.DTOs
{
    public record KnowledgeDto(Guid Id, string Title, string Content, string Category);
}