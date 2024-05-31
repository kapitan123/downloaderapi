namespace DocumentStore.Domain.Documents;

public interface IZipper
{
	Task<Stream> GetZipedFilesAsync(IEnumerable<Guid> fileIds, CancellationToken token);
}
