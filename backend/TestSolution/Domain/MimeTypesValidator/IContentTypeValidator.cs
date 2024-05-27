namespace DocumentStore.Domain.MimeTypesValidator;

public interface IContentTypeValidator
{
	bool IsSupported(string contentType);
	IEnumerable<string> SupportedTypes { get; }
}