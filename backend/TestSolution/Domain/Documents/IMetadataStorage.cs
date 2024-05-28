using TestSolution.Domain;

namespace DocumentStore.Domain.Documents;

public interface IMetadataStorage
{
	Task<List<DocumentMeta>> GetMetaOfAllDocuments(CancellationToken token);
}
