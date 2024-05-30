using DocumentStore.Domain;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Infrastructrue.DbPersistance;

public interface IPublicLinkRepository
{
	Task Save(PublicLink publicLink, CancellationToken token);

	Task<OneOf<PublicLink, NotFound>> Get(string publicId, CancellationToken token);

}
