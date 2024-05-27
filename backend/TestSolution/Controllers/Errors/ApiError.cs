namespace DocumentStore.Controllers.Errors;

public record ApiError(string Code, string Description)
{
	public static ApiError FileNotProvided() => new("file_not_provided", "");
}
