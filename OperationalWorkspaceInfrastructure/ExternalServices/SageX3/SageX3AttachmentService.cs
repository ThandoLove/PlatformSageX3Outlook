
using OperationalWorkspaceInfrastructure.Http;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3AttachmentService
{
    Task UploadAttachmentAsync(string entityId, string fileName, byte[] content, CancellationToken cancellationToken);
}

public class SageX3AttachmentService : ISageX3AttachmentService
{
    private readonly ISageHttpClient _client;
    private readonly string _baseUrl;

    public SageX3AttachmentService(ISageHttpClient client, string baseUrl)
    {
        _client = client;
        _baseUrl = baseUrl;
    }

    public async Task UploadAttachmentAsync(string entityId, string fileName, byte[] content, CancellationToken cancellationToken)
    {
        var payload = new { entityId, fileName, content = Convert.ToBase64String(content) };
        await _client.PostAsync($"{_baseUrl}/attachments", payload, cancellationToken);
    }
}