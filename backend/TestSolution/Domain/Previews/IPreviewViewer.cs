namespace DocumentStore.Domain.Preview;

public interface IPreviewViewer
{
	public Task<(Stream Content, long Size)> ViewForDocument(Guid documentId, CancellationToken token);
}
