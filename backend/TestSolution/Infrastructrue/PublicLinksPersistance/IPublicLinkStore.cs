using OneOf;
using OneOf.Types;

namespace DocumentStore.Infrastructrue.PublicLinksPersistance;

public interface IPublicLinkStore
{
	Task Save(string key, Guid value, int expirationInHours, CancellationToken token);

	Task<OneOf<Guid, NotFound>> Get(string key, CancellationToken token);
}
