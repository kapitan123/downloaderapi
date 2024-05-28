using DocumentStore.Domain.Preview;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.PreviewGenerator;

// I would use a https://docs.groupdocs.com/viewer/net/licensing-and-evaluation/
// It makes the task trivial, but API and documentation aint very good (looks like it requires a temp folder and so on)
// and I don't want to spend much time trying to troubleshoot it
// So I'll upload a dummy jpeg for each file
public class PreviewService(IPreviewContentStore contentStore) : IPreviewGenerator, IPreviewViewer
{
	public async Task GeneratePreview(Guid documentId, Stream _, string contentType, CancellationToken token)
	{
		using var emptyStream = new MemoryStream();
		await contentStore.SavePreviewAsync(documentId, emptyStream, token);
	}

	public Task<Stream> ViewForDocument(Guid documentId, CancellationToken token)
	{
		return contentStore.ReadPreviewAsync(documentId, token);
	}
}
