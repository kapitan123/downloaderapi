namespace DocumentStore.Domain.ShareabaleUrls;
using OneOf;
using OneOf.Types;

public interface IShareService
{
	Task<Uri> GetPublicUriFor(Guid documentId, int expirationInHours);

	Task<OneOf<Uri, Expired, NotFound>> GetDocumentByPublicId(Guid publicId);
}
