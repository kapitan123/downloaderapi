namespace DocumentStore.Infrastructrue.Persistance;

public interface ISharedLinksRepo
{
	public Task<string> GetDocumentByLink();
}
