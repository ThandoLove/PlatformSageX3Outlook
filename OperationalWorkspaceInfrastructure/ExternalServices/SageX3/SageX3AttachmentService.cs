using Microsoft.Extensions.Options;
using OperationalWorkspaceInfrastructure.Configuration;
using OperationalWorkspaceInfrastructure.Http;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public interface ISageX3AttachmentService
{
    Task UploadAttachmentAsync(
        string entityId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken);
}

public class SageX3AttachmentService : ISageX3AttachmentService
{
    private readonly ISageHttpClient _client;
    private readonly SageX3AttachmentOptions _options;

    public SageX3AttachmentService(
        ISageHttpClient client,
        IOptions<SageX3AttachmentOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task UploadAttachmentAsync(
        string entityId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            entityId,
            fileName,
            content = Convert.ToBase64String(content)
        };

        var url = $"{_options.BaseUrl.TrimEnd('/')}/attachments";

        await _client.PostAsync(url, payload, cancellationToken);
    }
}