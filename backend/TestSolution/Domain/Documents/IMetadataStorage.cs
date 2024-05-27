using TestSolution.Domain;

namespace DocumentStore.Domain.Documents;

public interface IMetadataStorage
{
	Task<DocumentMeta> GetMetaOfAllDocuments(CancellationToken token);
}
