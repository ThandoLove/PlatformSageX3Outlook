using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspace.Domain.Entities
{
    public sealed class Email
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string MessageId { get; set; } = string.Empty; // Outlook InternetMessageId
        public string Subject { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string BodyPreview { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
    }
}