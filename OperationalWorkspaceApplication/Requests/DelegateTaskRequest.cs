
namespace OperationalWorkspaceApplication.Requests
{

    public class DelegateTaskRequest
    {
        public Guid TaskId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
    }

}
