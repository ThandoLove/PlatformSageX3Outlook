using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Workspace;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OperationalWorkspaceUI.Components.Pages.Workspace.WorkspaceComponents
{
    public class AttachmentsBase : ComponentBase
    {
        [Inject] protected WorkspaceState State { get; set; } = null!;
        [Inject] protected NavigationManager Nav { get; set; } = null!;
        [Inject] protected HttpClient Http { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] protected IConfiguration Configuration { get; set; } = null!;
        [Inject] protected NotificationService Notifications { get; set; } = null!;
        [Inject] protected AttachmentUIService AttachmentService { get; set; } = null!;

        // 🔥 FIXED: Injected unified dashboard state container to access active context variables [INDEX]
        [Inject] protected DashboardState DashboardState { get; set; } = null!;

        protected string SearchQuery = "";
        protected string SelectedType = "All";
        protected int CurrentPage = 1;
        protected int PageSize = 10;
        protected AttachmentDto? SelectedDoc;

        protected IEnumerable<AttachmentDto> FilteredData => State.Attachments
            .Where(d => (SelectedType == "All" || d.ContentType == SelectedType) &&
                        (string.IsNullOrWhiteSpace(SearchQuery) || d.FileName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));

        protected IEnumerable<AttachmentDto> PaginatedDocs => FilteredData.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        protected int TotalPages => (int)Math.Ceiling((double)FilteredData.Count() / PageSize);

        protected void OpenPdfPreviewAsync()
        {
            if (SelectedDoc == null) return;
            Nav.NavigateTo($"/attachments/{SelectedDoc.Id}");
        }

        // ==========================================
        // 🏛️ GET PREVIEW STREAM URL FOR OBJECT FRAME
        // ==========================================
        protected string GetPdfUrl(AttachmentDto doc)
        {
            if (doc == null) return string.Empty;
            string options = "#toolbar=0&navpanes=0&statusbar=0&messages=0&scrollbar=0";

            bool useMockData = Configuration.GetValue<bool>("SageX3:UseMockData", true);

            if (!useMockData)
            {
                return $"/api/v1/Attachment/stream/{doc.Id}{options}";
            }

            // Maps cleanly to your local web root directory asset folder [INDEX]
            return $"/mock-documents/{doc.FileName}{options}";
        }


        // ==========================================
        // 📧 ATTACH CORRESPONDING FILE TO OUTLOOK EMAIL
        // ==========================================
        protected async Task AttachToEmail(AttachmentDto? doc)
        {
            if (doc == null) return;

            try
            {
                bool useMock = Configuration.GetValue<bool>("SageX3:UseMockData", true);
                string attachUrl;

                if (!useMock)
                {
                    try
                    {
                        var signed = await Http.GetFromJsonAsync<System.Text.Json.JsonElement>($"api/v1/Attachment/signed/{doc.Id}");
                        if (signed.ValueKind == System.Text.Json.JsonValueKind.Object && signed.TryGetProperty("Url", out var u))
                        {
                            attachUrl = u.GetString() ?? $"/api/v1/Attachment/stream/{doc.Id}";
                        }
                        else
                        {
                            attachUrl = $"/api/v1/Attachment/stream/{doc.Id}";
                        }
                    }
                    catch
                    {
                        attachUrl = $"/api/v1/Attachment/stream/{doc.Id}";
                    }
                }
                else
                {
                    // 🔥 FIXED: Dynamically appends your active base development host URI address at runtime [INDEX].
                    // This generates an absolute URL link, resolving the isolated Office.js attachment download block [INDEX].
                    string baseDevelopmentHostUrl = Nav.BaseUri.TrimEnd('/');
                    attachUrl = $"{baseDevelopmentHostUrl}/mock-documents/{doc.FileName}";
                }

                await JSRuntime.InvokeVoidAsync("officeBridge.attachDocument", attachUrl, doc.FileName);
                State.LogActivity(doc.FileName, "Attached to Email via Outlook Bridge", "Tia");
                State.Notify();
                Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Success, Summary = "Attached", Detail = $"{doc.FileName} attached to email context", Duration = 3000 });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AttachToEmail failed: {ex.Message}");
                Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Error, Summary = "Attachment Fault", Detail = "Could not package transaction file for Outlook dispatch.", Duration = 4000 });
            }
        }

        // ==========================================
        // 🏛️ BROWSE IMMUTABLE REPOSITORY INDEX FILE LIST
        // ==========================================
        protected async Task BrowseSageX3()
        {
            try
            {
                var fileNames = await AttachmentService.GetAttachmentsAsync();

                // 🔥 FIXED: Standardised content type mapping parameter to application/pdf 
                // so the browser reads it as a viewable file instead of a data stream download blob [INDEX]
                var mapped = fileNames.Select(name => new AttachmentDto(
                    Guid.NewGuid(),
                    name,
                    "application/pdf",
                    0,
                    name, // Kept clean to avoid duplicate relative prefix formatting faults [INDEX]
                    "SageX3",
                    DateTime.UtcNow))
                .ToList();

                State.Attachments = mapped;
                State.LogActivity("Sage Server Repository", "Queried ERP Document Index", "Tia");
                State.Notify();
                Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Info, Summary = "Sage Browse", Detail = $"Loaded {mapped.Count} documents from folder index", Duration = 3000 });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERP remote directory parsing failure: {ex.Message}");
                Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Error, Summary = "Sage Browse Error", Detail = "Failed to query Sage repository", Duration = 4000 });
            }
        }

        // ==========================================
        // 🔄 REFRESH ATTACHMENTS CACHE ENDPOINT
        // ==========================================
        protected async Task RefreshAttachmentsFromServer()
        {
            try
            {
                var response = await Http.GetAsync("api/v1/Attachment/list?ownerType=SageGlobalIndex&ownerId=All");

                if (response.IsSuccessStatusCode)
                {
                    await BrowseSageX3();
                    State.LogActivity("System Server", "Refreshed Attachments List Cache", "Tia");
                    State.Notify();
                }
                else if (Configuration.GetValue<bool>("SageX3:UseMockData", true))
                {
                    await BrowseSageX3();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Server refresh sync failed: {ex.Message}");
                await BrowseSageX3();
            }
        }

        // ==========================================
        // COMPLIANCE INTEGRATION SHIMS
        // ==========================================
        protected async Task HandleFileUpload(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file == null) return;

            State.SetUploading(true);
            try
            {
                using var content = new MultipartFormDataContent();
                var fileStream = file.OpenReadStream(10 * 1024 * 1024);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.Name);

                var response = await Http.PostAsync($"api/v1/Attachment/upload-file?ownerId=Tia&ownerType=DocumentList", content);

                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement?>();
                    Guid persistedId = Guid.NewGuid();
                    string fileUrl = $"/mock-documents/{file.Name}";

                    if (respJson.HasValue && respJson.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        if (respJson.Value.TryGetProperty("data", out var data) && data.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (data.TryGetProperty("Id", out var idVal) && idVal.TryGetGuid(out var parsedId)) persistedId = parsedId;
                            if (data.TryGetProperty("StoragePath", out var sp) && sp.ValueKind == System.Text.Json.JsonValueKind.String)
                                fileUrl = sp.GetString() ?? fileUrl;
                        }
                    }

                    var newDoc = new AttachmentDto(persistedId, file.Name, file.ContentType, file.Size, fileUrl, "Unlinked", DateTime.UtcNow);
                    State.Attachments.Insert(0, newDoc);
                    SelectedDoc = newDoc;

                    State.LogActivity(file.Name, "Uploaded Document", "Tia");
                    State.Notify();
                    Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Success, Summary = "Uploaded", Detail = $"{file.Name} uploaded (id: {persistedId})", Duration = 4000 });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload processing error: {ex.Message}");
            }
            finally
            {
                State.SetUploading(false);
            }
        }

        protected async Task HandleAttachDocument(AttachmentDto? doc)
        {
            if (doc == null) return;

            try
            {
                bool useMock = Configuration.GetValue<bool>("SageX3:UseMockData", true);

                string activeOwnerId = !string.IsNullOrEmpty(DashboardState.EmailContext?.BusinessPartnerCode)
                    ? DashboardState.EmailContext.BusinessPartnerCode
                    : "BPC-MOCK-101";

                var requestBody = new
                {
                    ownerType = "SageX3OrderContext",
                    ownerId = activeOwnerId,
                    fileName = doc.FileName,
                    contentType = doc.ContentType,
                    fileSize = doc.FileSize,
                    storagePath = useMock ? $"/mock-documents/{doc.FileName}" : $"/api/v1/Attachment/stream/{doc.Id}"
                };

                var response = await Http.PostAsJsonAsync("api/v1/Attachment/upload", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    State.LogActivity(doc.FileName, $"Attached to ERP ({activeOwnerId})", "Tia");
                    State.Notify();
                    Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Success, Summary = "Linked", Detail = $"Attached to record {activeOwnerId}", Duration = 4000 });
                }
                else if (useMock)
                {
                    State.LogActivity(doc.FileName, $"[Mock] Attached to ERP Record ({activeOwnerId})", "Tia");
                    State.Notify();
                    Notifications.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Success, Summary = "Linked (Mock Mode)", Detail = $"Associated document with entity: {activeOwnerId}", Duration = 4000 });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERP metadata linkage fault: {ex.Message}");
            }
        }

        protected async Task DeleteDocument(AttachmentDto? doc)
        {
            if (doc == null) return;

            State.Attachments.Remove(doc);
            if (SelectedDoc?.Id == doc.Id) SelectedDoc = null;

            State.LogActivity(doc.FileName, "Deleted Document", "Tia");
            State.Notify();
        }

        protected string FormatSize(long bytes) => bytes < 1048576 ? $"{(bytes / 1024.0):F1} KB" : $"{(bytes / 1048576.0):F1} MB";

        protected Icon GetIcon(string filename)
        {
            if (filename != null && filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) return new Icons.Regular.Size20.DocumentPdf();
            return new Icons.Regular.Size20.Document();
        }
    }
}
