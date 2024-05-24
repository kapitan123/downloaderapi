using TestSolution.Domain;

namespace DocumentStore.Domain.PreviewGenerator;

public interface IPreviewGenerator
{
	// AK TODO rename
	Task<Stream> GeneratePreview(Stream file, MimeType type);
}
