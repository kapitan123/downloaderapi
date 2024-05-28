namespace DocumentStore.Domain.ShareabaleUrls;
using OneOf;
using OneOf.Types;

public interface IShareService
{
	Task<string> GenerateTempPublicIdFor(Guid documentId, int expirationInHours, CancellationToken token);

	Task<OneOf<Guid, NotFound>> GetDocumentIdByPublicId(string publicId, CancellationToken token);
}