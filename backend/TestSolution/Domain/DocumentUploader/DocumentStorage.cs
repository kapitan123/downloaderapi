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
		// We can limit API to Get/Save and keep the increment logic in domain object
		// But the tradeoff would be a slow and complex concurrency control
		await metaRepo.IncrementDownloads(id);
		var meta = await metaRepo.GetAsync(id);

		var content = await store.ReadAsync(meta.ContentAddress, default);

		// AK TODO
		// We assume if we were able to successfully download a stream upsream services wont fail
		// We also assume that if it does fail, and an enduser doesn't get his file
		// We won't decrement a counter 

		// AK TODO if we implement a separate method to perform an atomic counter increment
		// the system will work faster
		// but i will refreain from this optimisation, as the 
		// AK TODO add atomar increment
		return (meta, content);
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

	/// <summary>
	/// Uploads document
	/// </summary>
	/// <param name="fileContent"></param>
	/// <param name="meta">metadata of the new document</param>
	/// <returns>updated metada</returns>
	public async Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token)
	{
		// AK Todo should be a single transaction
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
}
