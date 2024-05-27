using TestSolution.Domain;

namespace DocumentStore.Domain.DocumentUploader;

public interface IMetadataStorage
{
	Task<DocumentMeta> GetMetaOfAllDocuments();
}
