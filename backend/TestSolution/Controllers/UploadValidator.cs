using Microsoft.Extensions.Options;

namespace DocumentStore.Controllers;

public class UploadValidator(IOptions<UploadValidatorSettings> options) : IUploadValidator
{
	private readonly UploadValidatorSettings _settings = options.Value;
	public IEnumerable<string> SupportedTypes => throw new NotImplementedException();

	public long MaxSize => throw new NotImplementedException();

	public bool IsSupported(string contentType)
	{
		throw new NotImplementedException();
	}
}
