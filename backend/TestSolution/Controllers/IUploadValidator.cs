namespace DocumentStore.Controllers;

public interface IUploadValidator
{
	bool IsSupported(string contentType);

	IEnumerable<string> SupportedTypes { get; }

	long MaxSize { get; }
}