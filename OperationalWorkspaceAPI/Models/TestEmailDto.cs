namespace OperationalWorkspaceAPI.Models
{
    public class TestEmailDto
    {
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
