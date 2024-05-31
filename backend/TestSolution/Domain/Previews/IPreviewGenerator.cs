namespace DocumentStore.Domain.PreviewGenerator;

public interface IPreviewGenerator
{
	Task GeneratePreviewAsync(Guid documentId, Stream file, string contentType, CancellationToken token);
}
