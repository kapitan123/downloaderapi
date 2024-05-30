using DocumentStore.Domain;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Infrastructrue.DbPersistance;

public class PublicLinkRepository(DbContext context) : IPublicLinkRepository
{
	private readonly DbSet<PublicLink> publicLinks = context.Set<PublicLink>();

	public async Task Save(PublicLink publicLink, CancellationToken token)
	{
		await publicLinks.AddAsync(publicLink, token);
		await context.SaveChangesAsync(token);
	}

	public async Task<OneOf<PublicLink, NotFound>> Get(string publicId, CancellationToken token)
	{
		var doc = await publicLinks
			.FirstOrDefaultAsync(e => e.Id == publicId,
		token);

		return doc != null ? doc : new NotFound();
	}
}
