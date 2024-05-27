namespace DocumentStore.Domain.DocumentUploader;

public interface IZipper
{
	Task<Stream> GetZipedFiles(IEnumerable<Guid> fileIds, CancellationToken token);
}
