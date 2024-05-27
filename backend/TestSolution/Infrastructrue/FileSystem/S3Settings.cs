namespace DocumentStore.Infrastructrue.FileSystem;

public class S3Settings
{
	public static string Section => "S3Settings";
	public string DocumentsBucket { get; set; }

	public string PreviewsBucket { get; set; }
}
