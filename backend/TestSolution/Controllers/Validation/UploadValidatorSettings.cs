namespace DocumentStore.Controllers.Validation;

public class UploadValidatorSettings
{
	public static string Section => "UploadValidatorSettings";
	public long FileSizeLimitInBytes { get; set; }
	public required string[] AllowedContentTypes { get; set; }
}
