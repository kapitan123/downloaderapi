namespace DocumentStore.Domain.PreviewGenerator;

public class PreviewGenerator : IPreviewGenerator
{

	public Dictionary<string, string> keyValuePairs = new();
	public PreviewGenerator()
	{

	}

	public IEnumerable<string> SupportedTypes => throw new NotImplementedException();

	public Task GeneratePreview(Stream file, string contentType, CancellationToken token)
	{
		throw new NotImplementedException();
	}

	public bool IsSupported(string contentType)
	{
		throw new NotImplementedException();
	}
}
