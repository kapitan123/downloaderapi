using DocumentStore.Controllers.Errors;
using OneOf;
using OneOf.Types;

namespace DocumentStore.Controllers.Documents.Validation;

public interface IUploadValidator
{
	public OneOf<Success, ApiError> Validate(IFormFile file);
}