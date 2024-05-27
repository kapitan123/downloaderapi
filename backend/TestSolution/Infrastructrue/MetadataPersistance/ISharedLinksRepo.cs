namespace DocumentStore.Infrastructrue.MetadataPersistance;

public interface ISharedLinksRepo
{
    public Task<string> GetDocumentByLink();
}
