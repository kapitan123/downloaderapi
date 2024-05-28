using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Infrastructrue.MetadataPersistance;
using TestSolution.Domain;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.Documents;

public class DocumentStorage(IPreviewGenerator previewGenerator, IFileContentStore store, IMetadataRepository metaRepo)
	: IDocumentStorage, IMetadataStorage, IZipper
{
	public async Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token)
	{
		var metaTask = metaRepo.GetAsync(id);
		var contentTask = store.ReadAsync(id, token);

		Task.WaitAll([metaTask, contentTask], cancellationToken: token);

		// We can limit the API to Get/Save operations and keep the increment logic in the domain object.
		// However, the tradeoff would be slower performance and complex concurrency control.
		// Currently, we do not provide a rollback of an increment if an upstream service fails.
		// This functionality might be added later.
		// Also this feature can be based on a batch event processing, substentially reducing write load on the db.
		// Download events could also be used for analytics and forecasts.
		await metaRepo.IncrementDownloads(id);

		return (metaTask.Result, contentTask.Result);
	}

	/// <summary>
	/// Uploads document
	/// </summary>
	/// <param name="fileContent"></param>
	/// <param name="meta">metadata of the new document</param>
	/// <returns>updated metada</returns>
	public async Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token)
	{
		// We slightly optimise
		using var fsGenerator = new MemoryStream();
		using var fsStore = new MemoryStream();
		using var fsTee = new TeeStream(fsGenerator, fsStore);

		content.CopyTo(fsTee);

		// It is not clear if we should offload this generation to Lambda.
		// In the current implementation, we reuse the file stream which is already in memory, saving on I/O.
		// However, asynchronous processing would be a better solution for availability,
		// as preview generation is not a critical feature and can be retried in the background.
		var previewGenTask = previewGenerator.GeneratePreview(fsGenerator, meta.ContentType, token);

		var saveFileTask = store.SaveAsync(meta.Id, meta.ContentType, fsStore, token);

		// AK TODO add error swallowing for previewGenTask
		Task.WaitAll([previewGenTask, saveFileTask], cancellationToken: token);

		// It would make sense to emit an event after finishing the download.
		await metaRepo.SaveAsync(meta);
	}

	public Task<Stream> GetZipedFiles(IEnumerable<Guid> fileIds, CancellationToken token)
	{
		throw new NotImplementedException();

		//using var ms = new MemoryStream();
		//using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
		//{
		//	//QUery the Products table and get all image content  
		//	_dbcontext.Products.ToList().ForEach(file =>
		//	{
		//		var entry = zip.CreateEntry(file.ProImageName);
		//		using var fileStream = new MemoryStream(file.ProImageContent);
		//		using var entryStream = entry.Open();
		//		fileStream.CopyTo(entryStream);
		//	});
		//}
		//return ms;
	}

	public Task<List<DocumentMeta>> GetMetaOfAllDocuments(CancellationToken token)
	{
		throw new NotImplementedException();
	}
}
