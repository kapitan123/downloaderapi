using DocumentStore.Infrastructrue.PublicLinksPersistance;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Domain.ShareabaleUrls;

public class ShareService(IPublicLinkStore store) : IShareService
{
	// I prefer to specify all expected results as a union so the upstream service 
	// is forced to explicitly handle all of them
	public async Task<OneOf<Guid, NotFound>> GetDocumentIdByPublicId(string publicId, CancellationToken token)
	{
		return await store.Get(publicId, token);
	}

	public async Task<string> GenerateTempPublicIdFor(Guid documentId, int expirationInHours, CancellationToken token)
	{
		var newPublicId = "pub" + Guid.NewGuid();
		await store.Save(newPublicId, documentId, expirationInHours, token);

		return newPublicId;
	}
}
