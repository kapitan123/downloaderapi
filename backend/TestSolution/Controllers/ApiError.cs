namespace DocumentStore.Controllers;

public record ApiError(string Code, string Description);

public static class FileUploadError
{
	public static ApiError NotProvided() => new("file_not_provided", "Please provide a non empty file");

	public static ApiError TooBig(long limitInBytes) => new("file_is_too_big", $"File should not exceed {limitInBytes} bytes");

	public static ApiError ContentNotSupported(string contentType, IEnumerable<string> supportedTypes) =>
		new("file_type_not_supported", $"Contenttype {contentType} is not supported. " +
				$"Supported types are: {string.Join(" ,", supportedTypes)}");
}

public static class ZipDownloadError
{
	public static ApiError NotFilesToZip() => new("zip_empty", "Please provide at least two files to zip");

	public static ApiError TooManyFilesToZip(int maxFiles) => new("zip_too_many_files", $"Max files to zip is {maxFiles}");
}