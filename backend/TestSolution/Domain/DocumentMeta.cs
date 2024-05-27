namespace TestSolution.Domain;

// AK TODO should I try to do DDD at all?
public class DocumentMeta
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string ContentType { get; set; }

	public long Size { get; set; }

	public string PreviewAddress { get; set; }

	public DateTime UploadedOn { get; set; }

	public string ContentAddress { get; set; }

	public string UploadedBy { get; set; }

	public int DownloadsCount { get; set; } // AK TODO should be immutable for the Domain object

}
