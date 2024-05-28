namespace DocumentStore.Domain.ShareabaleUrls;
using OneOf;
using OneOf.Types;

public interface IShareService
{
	Task<Uri> GetPublicUriFor(Guid documentId, int expirationInHours, CancellationToken token);

	Task<OneOf<Guid, Expired, NotFound>> GetDocumentIdByPublicId(Guid publicId, CancellationToken token);
}