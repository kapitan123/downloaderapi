namespace TestSolution.Infrastructrue.Web;
public interface IFileContentStore
{
	Task<string> SaveFileAsync(Stream fileStream, CancellationToken token);

	Task<Stream> ReadFileAsync(string id, CancellationToken token);
}