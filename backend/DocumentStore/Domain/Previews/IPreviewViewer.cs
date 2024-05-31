namespace DocumentStore.Domain.Preview;

public interface IPreviewViewer
{
	public Task<Stream> ViewForDocument(Guid documentId, CancellationToken token);
}
