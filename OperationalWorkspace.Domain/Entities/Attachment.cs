
namespace OperationalWorkspace.Domain.Entities;

public class Attachment
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    // Use = null!; for properties that EF Core will populate
    public string OwnerType { get; private set; } = null!;
    public string OwnerId { get; private set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; private set; } = null!;
    public string StoragePath { get; private set; } = null!;
    public byte[] Content { get; set; } = null!;

    public long FileSize { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid EntityId { get; set; }

    public Attachment() { }

    public Attachment(
        string ownerType,
        string ownerId,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath,
        DateTime createdAt)
    {
        OwnerType = ownerType;
        OwnerId = ownerId;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        StoragePath = storagePath;
        CreatedAt = createdAt;
    }
}
