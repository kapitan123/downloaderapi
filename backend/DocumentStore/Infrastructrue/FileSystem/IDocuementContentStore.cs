namespace TestSolution.Infrastructrue.Web;
public interface IDocuementContentStore
{
	Task SaveDocumentAsync(Guid id, string contentType, Stream fileStream, CancellationToken token);

	Task<Stream> ReadDocumentAsync(Guid id, CancellationToken token);
}