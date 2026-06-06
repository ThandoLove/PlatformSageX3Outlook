
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    namespace OperationalWorkspaceUI.UIServices.Workspace
    {
    public class AttachmentUIService
        {
            private readonly List<string> _attachments = new();
            private readonly HttpClient _http;
            private readonly IConfiguration _configuration;

            public AttachmentUIService(HttpClient http, IConfiguration configuration)
            {
                _http = http;
                _configuration = configuration;
            }

            /// <summary>
            /// Returns a list of attachment file names. If SageX3 is configured (UseMockData=false)
            /// this will call the backend API to retrieve attachments, otherwise a small mock set is returned.
            /// </summary>
            public async Task<List<string>> GetAttachmentsAsync()
            {
                // If we already have cached entries return them immediately
                if (_attachments.Any()) return _attachments.ToList();

                bool useMock = _configuration.GetValue<bool>("SageX3:UseMockData");

                if (useMock)
                {
                    // Provide a friendly mock payload for local development and tests
                    _attachments.AddRange(new[] { "mock-invoice.pdf", "mock-proposal.docx", "mock-image.png" });
                    return _attachments.ToList();
                }

                try
                {
                    // Query the server attachment index. The API returns an AttachmentListResponse
                    var resp = await _http.GetFromJsonAsync<OperationalWorkspaceApplication.Responses.AttachmentListResponse>("api/v1/Attachment/list?ownerType=SageGlobalIndex&ownerId=All");
                    if (resp?.Attachments != null)
                    {
                        foreach (var a in resp.Attachments)
                        {
                            if (!string.IsNullOrWhiteSpace(a.FileName) && !_attachments.Contains(a.FileName))
                                _attachments.Add(a.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If the backend fails, fall back to local mock data so UI remains functional
                    global::System.Diagnostics.Debug.WriteLine($"AttachmentUIService: backend fetch failed: {ex.Message}");
                    _attachments.AddRange(new[] { "mock-invoice.pdf", "mock-proposal.docx" });
                }

                return _attachments.ToList();
            }

            public Task AddAttachmentAsync(string attachment)
            {
                if (!string.IsNullOrWhiteSpace(attachment) && !_attachments.Contains(attachment))
                    _attachments.Insert(0, attachment);

                return Task.CompletedTask;
            }
        }
    }