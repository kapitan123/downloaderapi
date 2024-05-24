namespace DocumentStore.Domain;

public class PublicLink
{
	public Guid Id { get; set; }
	public Guid DocumentId { get; set; }

	public string Url { get; set; }

	public DateTime ExpiresOnUtc { get; set; }
}
