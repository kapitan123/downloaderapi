namespace DocumentStore.Domain.Documents;

public interface IMetadataStorage
{
	Task<List<DocumentMeta>> GetMetaOfAllDocumentsAsync(CancellationToken token);
}
