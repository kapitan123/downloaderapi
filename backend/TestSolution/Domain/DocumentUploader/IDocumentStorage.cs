using TestSolution.Domain;

namespace DocumentStore.Domain.DocumentUploader;
public interface IDocumentStorage
{
	Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token);
	Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token);
}