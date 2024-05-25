namespace DocumentStore.Domain.PreviewGenerator;

public interface IPreviewGenerator
{
	// AK TODO rename, maybe create a separate class
	// this shit is supposed to be a separated service or at least a project

	Task<string> GeneratePreview(Stream file, string contentType, CancellationToken token);
}
