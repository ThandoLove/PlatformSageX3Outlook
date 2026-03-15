using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspace.Domain.Entities
{
    public sealed class Knowledge
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

