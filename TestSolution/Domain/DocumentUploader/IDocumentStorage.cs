using TestSolution.Domain;

namespace DocumentStore.Domain.DocumentUploader;
public interface IDocumentStorage
{
	Task<DocumentMeta> UploadNew(Stream fileContent, DocumentMeta meta, CancellationToken token);
}