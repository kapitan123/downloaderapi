using DocumentStore.Infrastructrue.DbPersistance;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Domain.ShareabaleUrls;

public class ShareService(IPublicLinkRepository repo) : IShareService
{
	// I prefer to specify all expected results as a union so the upstream service is forced to explicitly handle all of them
	// for example if we introduce a new error for AccessRestricted results like this OneOf<Guid, Expired, AccessRestricted, NotFound>
	// app will not compile until we add handling logic in consumers
	// in my opinion this pattern is more explicit than a Result monad, but sometimes the list of expected types gets too long
	public async Task<OneOf<Guid, Expired, NotFound>> GetDocumentIdByPublicId(string publicId, CancellationToken token)
	{
		var result = await repo.Get(publicId, token);

		return result.Match<OneOf<Guid, Expired, NotFound>>(
			// DateTime.UtcNo should be injected with TimeProvider
			pl => pl.ExpiresOn > DateTime.UtcNow ? pl.DocumentId : new Expired(),
			notFound => notFound);
	}

	public async Task<string> GenerateTempPublicIdFor(Guid documentId, int expirationInHours, CancellationToken token)
	{
		var newPublicId = "pub-" + Guid.NewGuid();

		var publicLink = new PublicLink
		{
			Id = newPublicId,
			DocumentId = documentId,
			CreatedOn = DateTime.UtcNow,
			// As timeframes are big we don't care about minor clock skew
			ExpiresOn = DateTime.UtcNow.AddHours(expirationInHours),
		};

		await repo.Save(publicLink, token);

		return newPublicId;
	}
}

public record Expired();