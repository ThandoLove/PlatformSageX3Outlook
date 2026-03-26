
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace OperationalWorkspaceUI.UIServices.Workspace
    {
    public class AttachmentUIService
        {
            private readonly List<string> _attachments = new();

            public Task<List<string>> GetAttachmentsAsync()
            {
                return Task.FromResult(_attachments);
            }

            public Task AddAttachmentAsync(string attachment)
            {
                _attachments.Add(attachment);
                return Task.CompletedTask;
            }
        }
    }