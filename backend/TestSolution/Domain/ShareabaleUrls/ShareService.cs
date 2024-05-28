using DocumentStore.Infrastructrue.PublicLinksPersistance;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Domain.ShareabaleUrls;

public class ShareService(IPublicLinkStore store) : IShareService
{
	// I prefer to specify all expected results as a union so the upstream service 
	// is forced to explicitly handle all of them
	// for example if we introduce a new error for Expired results like this OneOf<Guid, Expired, NotFound>
	// app will not compile until we add handling logic in consumers
	public async Task<OneOf<Guid, NotFound>> GetDocumentIdByPublicId(string publicId, CancellationToken token)
	{
		return await store.Get(publicId, token);
	}

	public async Task<string> GenerateTempPublicIdFor(Guid documentId, int expirationInHours, CancellationToken token)
	{
		var newPublicId = "pub-" + Guid.NewGuid();
		await store.Save(newPublicId, documentId, expirationInHours, token);

		return newPublicId;
	}
}
