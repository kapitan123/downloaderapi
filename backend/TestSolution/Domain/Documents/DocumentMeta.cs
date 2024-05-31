namespace DocumentStore.Domain.Documents;

// It has no domain logic, so I skipped separating DB and Domain models
public record DocumentMeta
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string ContentType { get; set; }

	public long Size { get; set; }

	public DateTime UploadedOn { get; set; }

	public string UploadedBy { get; set; }

	public int DownloadsCount { get; set; }

	public void IncrementDownloads()
	{
		DownloadsCount++;
	}

}
