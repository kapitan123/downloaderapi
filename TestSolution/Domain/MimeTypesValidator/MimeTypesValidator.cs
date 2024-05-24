namespace DocumentStore.Domain.MimeTypesValidator;

// AK TODO this shit can be a part of preview Generator
public class MimeTypesValidator : IContentTypeValidator
{
	public IEnumerable<string> SupportedTypes => new List<string>();
	public bool IsSupported(string contentType)
	{
		return true;
	}
}
