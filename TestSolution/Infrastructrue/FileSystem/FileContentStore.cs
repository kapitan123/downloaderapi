namespace TestSolution.Infrastructrue.Web;
public interface FileContentStore
{
	Task UpsertFile(Stre, CancellationToken token);

	Task<Stream> ReadFile(string documentPath, CancellationToken token);
}