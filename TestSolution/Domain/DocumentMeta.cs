namespace TestSolution.Domain;

public class DocumentMeta
{
	public Guid Id { get; set; }
	public string Name { get; set; }

	public MimeType Type { get; set; }

	public string PreviewAddress { get; set; }

	public DateTime UploadedOn { get; set; }

	public int DownloadedCount { get; set; }

	public string ContentAddress { get; set; }

	public string Version { get; set; }
}
