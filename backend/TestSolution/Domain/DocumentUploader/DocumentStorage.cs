using DocumentStore.Domain.PreviewGenerator;
using TestSolution.Domain;
using TestSolution.Infrastructrue.Persistance;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.DocumentUploader;

public class DocumentStorage(IPreviewGenerator previewGenerator, IFileContentStore store, IMetadataRepository metaRepo)
	: IDocumentStorage, IMetadataStorage, IZipper
{
	public async Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token)
	{
		var meta = await metaRepo.GetAsync(id);
		var content = await store.ReadAsync(meta.ContentAddress, default);

		// We can limit the API to Get/Save operations and keep the increment logic in the domain object.
		// However, the tradeoff would be slower performance and complex concurrency control.
		// Currently, we do not provide a rollback of an increment if an upstream service fails.
		// This functionality might be added later.
		await metaRepo.IncrementDownloads(id);

		return (meta, content);
	}

	/// <summary>
	/// Uploads document
	/// </summary>
	/// <param name="fileContent"></param>
	/// <param name="meta">metadata of the new document</param>
	/// <returns>updated metada</returns>
	public async Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token)
	{
		using var fsGenerator = new MemoryStream();
		using var fsStore = new MemoryStream();
		using var fsTee = new TeeStream(fsGenerator, fsStore);

		content.CopyTo(fsTee);

		var previewGenTask = previewGenerator.GeneratePreview(fsGenerator, meta.ContentType, token);

		var saveFileTask = store.SaveAsync(fsStore, token);

		Task.WaitAll([previewGenTask, saveFileTask], cancellationToken: token);

		// AK TODO probably I should not mutate it
		// Add error handling to not fail if preview gen fails
		meta.ContentAddress = saveFileTask.Result;

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

	public Task<DocumentMeta> GetMetaOfAllDocuments()
	{
		throw new NotImplementedException();
	}
}
