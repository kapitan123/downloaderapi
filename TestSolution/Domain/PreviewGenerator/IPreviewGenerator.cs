using TestSolution.Domain;

namespace DocumentStore.Domain.PreviewGenerator;

public interface IPreviewGenerator
{
	// AK TODO rename, maybe create a separate class
	// this shit is supposed to be a separated service or at least a project

	Task<Stream> GeneratePreview(Stream file, MimeType type);
}
