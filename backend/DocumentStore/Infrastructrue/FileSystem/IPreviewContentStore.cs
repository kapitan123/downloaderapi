namespace TestSolution.Infrastructrue.Web;
public interface IPreviewContentStore
{
	Task SavePreviewAsync(Guid id, Stream fileStream, CancellationToken token);

	Task<Stream> ReadPreviewAsync(Guid id, CancellationToken token);
}