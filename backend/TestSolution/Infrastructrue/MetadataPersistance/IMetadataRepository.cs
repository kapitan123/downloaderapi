using TestSolution.Domain;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public interface IMetadataRepository
{
	public Task<DocumentMeta> GetAsync(Guid id, CancellationToken token);

	// Ak it is making api more complex, but it will be a lot simplier to achive concurrency
	// we can still do read/update/save to keep Domain in one place, so its a tradeoff
	public Task IncrementDownloads(Guid id, CancellationToken token);

	// AK TODO fetch all docs without paging
	public Task<List<DocumentMeta>> GetAllAsync(CancellationToken token);

	// AK TODO we can use a Result type
	public Task SaveAsync(DocumentMeta document, CancellationToken token);
}
