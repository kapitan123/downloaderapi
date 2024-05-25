using DocumentStore.Domain.PreviewGenerator;
using TestSolution.Domain;
using TestSolution.Infrastructrue.Persistance;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.DocumentUploader;

public class DocumentStorage(IPreviewGenerator previewGenerator, IFileContentStore store, IMetadataRepository metaRepo) : IDocumentStorage
{
	private readonly IPreviewGenerator _previewGenerator = previewGenerator;
	private readonly IFileContentStore _store = store;
	private readonly IMetadataRepository _metaRepo = metaRepo;
	/// <summary>
	/// Uploads document
	/// </summary>
	/// <param name="fileContent"></param>
	/// <param name="meta">metadata of the new document</param>
	/// <returns>updated metada</returns>
	public async Task<DocumentMeta> UploadNew(Stream fileContent, DocumentMeta meta, CancellationToken token)
	{
		// AK Todo should be a single transaction
		using var fsGenerator = new MemoryStream();
		using var fsStore = new MemoryStream();
		using var fsTee = new TeeStream(fsGenerator, fsStore);

		var previewGenTask = _previewGenerator.GeneratePreview(fsGenerator, meta.Type, token);

		var saveFileTask = _store.SaveFileAsync(fsStore, token);

		Task.WaitAll([previewGenTask, saveFileTask], cancellationToken: token);

		// AK TODO probably I should not mutate it
		// Add error handling to not fail if preview gen fails
		meta.PreviewAddress = previewGenTask.Result;
		meta.ContentAddress = saveFileTask.Result;

		await _metaRepo.SaveAsync(meta);

		return meta;
	}
}
