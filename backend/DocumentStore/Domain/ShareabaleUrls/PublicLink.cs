namespace DocumentStore.Domain.ShareabaleUrls;

public class PublicLink
{
	public string Id { get; set; }

	public Guid DocumentId { get; set; }

	public DateTime CreatedOn { get; set; }

	public DateTime ExpiresOn { get; set; }
}
