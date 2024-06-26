﻿using DocumentStore.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public class MetadataRepository(DocumentsStoreDbContext context) : IMetadataRepository
{
	private readonly DbSet<DocumentMeta> documentMetas = context.DocumentMetas;

	public async Task<List<DocumentMeta>> GetAllAsync(CancellationToken token)
	{
		return await documentMetas.ToListAsync(token);
	}

	public async Task<DocumentMeta> GetAsync(Guid id, CancellationToken token)
	{
		var doc = await documentMetas
			.FirstOrDefaultAsync(e => e.Id == id,
		token);

		// I'm not a big fan of this approach but this is how it's done most of the time
		// or return of a nullable.
		return doc ?? throw new KeyNotFoundException();
	}

	// This method does relies on a proper transaction isolation level
	// and would not work with the default Read_Comitted
	// appliaction level locking would not help if we plan to have any kind of redundancy 
	public async Task IncrementDownloadsAsync(Guid id, CancellationToken token)
	{
		try
		{
			using var transaction = context.Database.BeginTransaction();

			var doc = await documentMetas
				.FirstAsync(e => e.Id == id,
			token);

			doc.IncrementDownloads();

			documentMetas.Update(doc);
			await context.SaveChangesAsync(token);

			transaction.Commit();
		}
		catch (DbUpdateConcurrencyException)
		{
			var test = "";
			// we should have some kind of a retry logic here
		}
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
		catch (DbUpdateConcurrencyException)
		{
			// we should have some kind of a retry logic here
		}
	}
}
