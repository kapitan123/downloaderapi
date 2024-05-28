using DocumentStore.Controllers.Errors;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Controllers.Documents.Validation;

public class UploadValidator(IOptions<UploadValidatorSettings> options) : IUploadValidator
{
	private readonly UploadValidatorSettings _settings = options.Value;

	// It's my preference to use OneOf in cases like this.
	// Though I would stick to patterns established in an existing codebase.
	public OneOf<Success, ApiError> Validate(IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return FileUploadError.NotProvided();
		}

		if (file.Length > _settings.FileSizeLimitInBytes)
		{
			return FileUploadError.TooBig(_settings.FileSizeLimitInBytes);
		}

		var type = file.FileName[(file.FileName.LastIndexOf('.') + 1)..];

		// It is based on file estensions as any binary data can be represented by octet stream for example
		if (!_settings.AllowedContentTypes.Contains(type))
		{
			return FileUploadError.ContentNotSupported(file.ContentType, _settings.AllowedContentTypes);
		}

		return new Success();
	}
}
