﻿using System.IO.Compression;
using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Infrastructrue.MetadataPersistance;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.Documents;

public class DocumentStorage(IPreviewGenerator previewGenerator, IDocuementContentStore store, IMetadataRepository metaRepo)
	: IDocumentStorage, IMetadataStorage, IZipper
{
	public async Task<(DocumentMeta Meta, Stream Content)> GetAsync(Guid id, CancellationToken token)
	{
		var metaTask = metaRepo.GetAsync(id, token);
		var contentTask = store.ReadDocumentAsync(id, token);

		Task.WaitAll([metaTask, contentTask], cancellationToken: token);

		// We can limit the API to Get/Save operations and keep the increment logic in the domain object.
		// However, the tradeoff would be slower performance and complex concurrency control.
		// Currently, we do not provide a rollback of an increment if an upstream service fails.
		// This functionality might be added later.
		// Also this feature can be based on a batch event processing, substentially reducing write load on the db.
		// Download events could also be used for analytics and forecasts.
		await metaRepo.IncrementDownloads(id, token);

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
		var previewGenTask = previewGenerator.GeneratePreview(meta.Id, fsGenerator, meta.ContentType, token);

		var saveFileTask = store.SaveDocumentAsync(meta.Id, meta.ContentType, fsStore, token);

		Task.WaitAll([previewGenTask, saveFileTask], cancellationToken: token);

		// It would make sense to emit an event after finishing the download.
		await metaRepo.SaveAsync(meta, token);
	}

	public async Task<Stream> GetZipedFiles(IEnumerable<Guid> fileIds, CancellationToken token)
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

	public async Task<List<DocumentMeta>> GetMetaOfAllDocuments(CancellationToken token)
	{
		var result = await metaRepo.GetAllAsync(token);

		return result;
	}
}
