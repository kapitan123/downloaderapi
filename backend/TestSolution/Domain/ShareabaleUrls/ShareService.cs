using OneOf;
using OneOf.Types;

namespace DocumentStore.Domain.ShareabaleUrls;

public class ShareService : IShareService
{
	// I prefer to specify all expected results as a union so the upstream service 
	// is forced to explicitly handle all of them
	public Task<OneOf<Guid, Expired, NotFound>> GetDocumentIdByPublicId(Guid publicId, CancellationToken token)
	{
		throw new NotImplementedException();
	}

	public Task<Uri> GetPublicUriFor(Guid documentId, int expirationInHours, CancellationToken token)
	{
		throw new NotImplementedException();
	}
}
