using System.IO.Compression;
using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Infrastructrue.MetadataPersistance;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.Documents;

public class DocumentStorage(IPreviewGenerator previewGenerator, IDocuementContentStore store, IMetadataRepository metaRepo, ILogger<DocumentStorage> logger)
	: IDocumentStorage, IMetadataStorage, IZipper
{
	public async Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token)
	{
		var metaTask = metaRepo.GetAsync(id, token);
		var contentTask = store.ReadDocumentAsync(id, token);

		await Task.WhenAll([metaTask, contentTask]);

		// We can limit the API to Get/Save operations and keep the increment logic in the domain object.
		// However, the tradeoff would be slower performance and complex concurrency control.
		// Currently, we do not provide a rollback of an increment if an upstream service fails.
		// This functionality might be added later.
		// Also this feature can be based on a batch event processing, substentially reducing write load on the db.
		// Download events could also be used for analytics and forecasts.
		await metaRepo.IncrementDownloadsAsync(id, token);

		return (metaTask.Result, contentTask.Result);
	}

	public async Task SaveAsync(DocumentMeta meta, Stream content, CancellationToken token)
	{
		// We slightly optimise removing repeated stream reads
		using var fsGenerator = new MemoryStream();
		using var fsStore = new MemoryStream();
		using var fsTee = new TeeStream(fsGenerator, fsStore);

		content.CopyTo(fsTee);

		// It is not clear if we should offload this generation to Lambda.
		// In the current implementation, we reuse the file stream which is already in memory, saving on I/O.
		// However, asynchronous processing would be a better solution for availability,
		// as preview generation is not a critical feature and can be retried in the background.
		var previewGenTask = previewGenerator.GeneratePreviewAsync(meta.Id, fsGenerator, meta.ContentType, token);

		var saveFileTask = store.SaveDocumentAsync(meta.Id, meta.ContentType, fsStore, token);

		await Task.WhenAll([previewGenTask, saveFileTask]);

		// It would make sense to emit an event after finishing the download.
		await metaRepo.SaveAsync(meta, token);
	}

	public async Task<Stream> GetZipedFilesAsync(IEnumerable<Guid> fileIds, CancellationToken token)
	{
		var documentTasks = fileIds.Select(id => GetAsync(id, token)).ToList();

		using var ms = new MemoryStream();

		using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
		{
			var documents = await Task.WhenAll(documentTasks);

			foreach (var (meta, content) in documents)
			{
				var entry = zip.CreateEntry(meta.Name, CompressionLevel.Optimal);

				using var entryStream = entry.Open();
				content.Seek(0, SeekOrigin.Begin);
				await content.CopyToAsync(entryStream, token);
			}
		}

		// Reset stream to start to allow reads upstream
		ms.Seek(0, SeekOrigin.Begin);

		return ms;
	}

	public async Task<List<DocumentMeta>> GetMetaOfAllDocumentsAsync(CancellationToken token)
	{
		var result = await metaRepo.GetAllAsync(token);

		return result;
	}

	public async Task<(DocumentMeta Meta, Stream Content)> GetFilteredByUserAsync(Guid id, string user, CancellationToken token)
	{
		// This operation can be performed on the Database side
		// But this way we won't be able to tell if the user has no access
		// or no file with such id
		var meta = await metaRepo.GetAsync(id, token);

		if (meta.UploadedBy != user)
		{
			// This is not a great way to do it, because this exception message can't be used in structural logging, hence params won't be indexed
			// But to do it a proper way I would need to introduce a custom exception.			
			throw new UnauthorizedAccessException($"User {user} tried to access a file {id} which belongs to {meta.UploadedBy}");
		}

		var content = await store.ReadDocumentAsync(id, token);

		await metaRepo.IncrementDownloadsAsync(id, token);

		return (meta, content);

	}
}
