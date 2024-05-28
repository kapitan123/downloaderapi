namespace DocumentStore.Domain.PreviewGenerator;

public interface IPreviewGenerator
{
	Task GeneratePreview(Guid documentId, Stream file, string contentType, CancellationToken token);
}
