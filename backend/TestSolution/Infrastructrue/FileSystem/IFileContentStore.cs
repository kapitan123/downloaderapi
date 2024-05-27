namespace TestSolution.Infrastructrue.Web;
public interface IFileContentStore
{
	Task<string> SaveAsync(Stream fileStream, CancellationToken token);

	Task<Stream> ReadAsync(string id, CancellationToken token);
}