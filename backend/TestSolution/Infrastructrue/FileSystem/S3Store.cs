using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.Infrastructrue.FileSystem;

public class S3Store(
	IAmazonS3 s3Client,
	ITransferUtility transferUtility,
	IOptions<S3Settings> options,
	ILogger<S3Store> logger) : IFileContentStore
{
	private readonly string _bucket = options.Value.DocumentsBucket;

	public async Task SaveAsync(Guid id, string contentType, Stream fileStream, CancellationToken token)
	{
		var uploadRequest = new TransferUtilityUploadRequest
		{
			InputStream = fileStream,
			ContentType = contentType,
			BucketName = _bucket,
			Key = id.ToString(),
			AutoCloseStream = true
		};

		await transferUtility.UploadAsync(uploadRequest, token);
	}

	public async Task<Stream> ReadAsync(Guid id, CancellationToken token)
	{
		var request = new GetObjectRequest
		{
			BucketName = _bucket,
			Key = id.ToString()
		};

		// AK TODO this shit might be disposed
		using var response = await s3Client.GetObjectAsync(request, token);
		return response.ResponseStream;
	}
}