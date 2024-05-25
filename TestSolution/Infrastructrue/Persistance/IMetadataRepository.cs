using TestSolution.Domain;

namespace TestSolution.Infrastructrue.Persistance;

public interface IMetadataRepository
{
	public Task<DocumentMeta> GetAsync(string id);

	// AK TODO fetch all docs without paging
	public Task<List<DocumentMeta>> GetAllAsync();

	// AK TODO we can use a Result type
	public Task SaveAsync(DocumentMeta document);
}
