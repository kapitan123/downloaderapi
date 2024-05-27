namespace TestSolution.Infrastructrue.Web;
public interface IFileContentStore
{
	Task SaveAsync(Guid id, string contentType, Stream fileStream, CancellationToken token);

	Task<Stream> ReadAsync(Guid id, CancellationToken token);
}