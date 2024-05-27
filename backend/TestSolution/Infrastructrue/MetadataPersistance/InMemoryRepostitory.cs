using TestSolution.Domain;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

// AK Todo it is explicitly configured 
public class InMemoryRepostitory(TestDbContext context) : IMetadataRepository
{
	public Task<List<DocumentMeta>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<DocumentMeta> GetAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task IncrementDownloads(Guid id)
	{
		throw new NotImplementedException();
	}

	public Task SaveAsync(DocumentMeta document)
	{
		throw new NotImplementedException();
	}
}
