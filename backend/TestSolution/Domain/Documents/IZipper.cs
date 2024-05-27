namespace DocumentStore.Domain.Documents;

public interface IZipper
{
	Task<Stream> GetZipedFiles(IEnumerable<Guid> fileIds, CancellationToken token);
}
