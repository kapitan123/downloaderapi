using TestSolution.Domain;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public interface IMetadataRepository
{
	public Task<DocumentMeta> GetAsync(Guid id, CancellationToken token);

	public Task IncrementDownloads(Guid id, CancellationToken token);

	public Task<List<DocumentMeta>> GetAllAsync(CancellationToken token);

	public Task SaveAsync(DocumentMeta document, CancellationToken token);
}
