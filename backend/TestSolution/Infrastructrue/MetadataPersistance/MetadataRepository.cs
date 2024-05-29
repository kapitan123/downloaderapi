using Microsoft.EntityFrameworkCore;
using TestSolution.Domain;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

// AK Todo it is explicitly configured 
public class MetadataRepository(DocumentsMetaDbContext context) : IMetadataRepository
{
	private readonly DbSet<DocumentMeta> documentMetas = context.Set<DocumentMeta>();

	public Task<List<DocumentMeta>> GetAllAsync(CancellationToken token)
	{
		throw new NotImplementedException();
	}

	public async Task<DocumentMeta> GetAsync(Guid id, CancellationToken token)
	{
		var doc = await documentMetas
			.FirstOrDefaultAsync(e => e.Id == id,
		token);

		// I'm not a big fan of this approach but this is how ti's done most of the time
		return doc ?? throw new KeyNotFoundException();
	}

	// This method does relies on a proper transaction isolation level
	// and would not work with the default Read_Comitted
	public async Task IncrementDownloads(Guid id, CancellationToken token)
	{
		using var transaction = context.Database.BeginTransaction();

		var doc = await documentMetas
			.FirstAsync(e => e.Id == id,
		token);

		doc.IncrementDownloads();

		documentMetas.Update(doc);
		await context.SaveChangesAsync(token);
	}

	public async Task SaveAsync(DocumentMeta document, CancellationToken token)
	{
		try
		{
			context.Entry(document).State = document.Id == Guid.Empty ?
								   EntityState.Added :
								   EntityState.Modified;

			await documentMetas.AddAsync(document, token);
			await context.SaveChangesAsync(token);
		}
		catch (DbUpdateConcurrencyException exception)
		{
			// we should have some kind of a retry logic here
		}
	}
}
