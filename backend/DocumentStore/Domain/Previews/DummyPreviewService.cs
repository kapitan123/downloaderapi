using DocumentStore.Domain.Preview;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Domain.PreviewGenerator;

// I would use a https://docs.groupdocs.com/viewer/net/licensing-and-evaluation/
// but API and documentation aint very good (looks like it requires a temp folder and so on)
// It's not complex but time consuming
// So I'll upload a dummy jpeg for each file
public class DummyPreviewService(IPreviewContentStore contentStore) : IPreviewGenerator, IPreviewViewer
{
	public async Task GeneratePreviewAsync(Guid documentId, Stream stream, string contentType, CancellationToken token)
	{
		// dummy previews work only with image/jpeg
		await contentStore.SavePreviewAsync(documentId, stream, token);
	}

	public Task<Stream> ViewForDocument(Guid documentId, CancellationToken token)
	{
		return contentStore.ReadPreviewAsync(documentId, token);
	}
}
