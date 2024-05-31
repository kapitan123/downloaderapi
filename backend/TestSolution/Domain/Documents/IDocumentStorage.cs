namespace DocumentStore.Domain.Documents;
public interface IDocumentStorage
{
	Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token);

	Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token);

	Task<(DocumentMeta Meta, Stream Content)> GetFilteredByUserAsync(Guid id, string user, CancellationToken token);
}