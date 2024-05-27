using TestSolution.Domain;

namespace TestSolution.Infrastructrue.Persistance;

public interface IMetadataRepository
{
	public Task<DocumentMeta> GetAsync(Guid id);

	// Ak it is making api more complex, but it will be a lot simplier to achive concurrency
	// we can still do read/update/save to keep Domain in one place, so its a tradeoff
	public Task IncrementDownloads(Guid id);

	// AK TODO fetch all docs without paging
	public Task<List<DocumentMeta>> GetAllAsync();

	// AK TODO we can use a Result type
	public Task SaveAsync(DocumentMeta document);
}
