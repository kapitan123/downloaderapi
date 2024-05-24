namespace TestSolution.Infrastructrue.Web;
public interface IFileContentStore
{
	// 
	Task<string> SaveFileAsync(Stream fileStream, string fileName);

	Task<Stream> ReadFileAsync(string id);
}