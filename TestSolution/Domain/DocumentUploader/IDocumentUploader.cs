using TestSolution.Domain;

namespace DocumentStore.Domain.DocumentUploader;
public interface IDocumentUploader
{
	Task<DocumentMeta> UploadNew(Stream fileContent, DocumentMeta meta);
}