using TestSolution.Domain;

namespace DocumentStore.Domain.DocumentUploader;

public class DocumentUploader : IDocumentUploader
{
	/// <summary>
	/// Uploads document
	/// </summary>
	/// <param name="fileContent"></param>
	/// <param name="meta">metadata of the new document</param>
	/// <returns>updated metada</returns>
	public Task<DocumentMeta> UploadNew(Stream fileContent, DocumentMeta meta) // AK TODO can be separated
	{
		throw new NotImplementedException();
	}
}
