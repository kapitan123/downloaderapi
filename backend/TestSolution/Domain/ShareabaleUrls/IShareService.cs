namespace DocumentStore.Domain.ShareabaleUrls;

public interface IShareService
{
	Task<Uri> GetPublicUriFor(Guid documentId, int expirationInHours);
}
