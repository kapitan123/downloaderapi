using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using DocumentStore.Controllers.Documents.Validation;
using DocumentStore.Domain.Documents;
using DocumentStore.Domain.Preview;
using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Domain.ShareabaleUrls;
using DocumentStore.Infrastructrue.DbPersistance;
using DocumentStore.Infrastructrue.FileSystem;
using DocumentStore.Infrastructrue.MetadataPersistance;
using Microsoft.EntityFrameworkCore;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.ServiceCollectionExtensions;

// I put serviceExtensions in the api layer
// Because I grouped code by layers
// Though generally I prefer grouping code by features, because this way you can trully chive a pluggable architecture
public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddInfra(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<SqlSettings>(
			builder.Configuration.GetSection(SqlSettings.Section));

		builder.Services.Configure<S3Settings>(
			builder.Configuration.GetSection(S3Settings.Section));

		var awsOptions = builder.Configuration.GetAWSOptions();

		var awsCredentials = new BasicAWSCredentials("test", "test");

		var s3Settings = builder.Configuration.GetSection(S3Settings.Section).Get<S3Settings>();

		builder.Services.AddSingleton<IAmazonS3>(sp =>
		{
			return new AmazonS3Client(awsCredentials, new AmazonS3Config
			{
				ServiceURL = s3Settings.ServiceUrl,
				UseHttp = true,
				ForcePathStyle = true
			});
		});

		var sqlSettings = builder.Configuration.GetSection(SqlSettings.Section).Get<SqlSettings>();
		builder.Services.AddDbContext<DocumentsStoreDbContext>(options =>
			options.UseNpgsql(sqlSettings.ConnectionString));

		builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
		builder.Services.AddScoped<IPublicLinkRepository, PublicLinkRepository>();

		builder.Services.AddSingleton<ITransferUtility, TransferUtility>();
		builder.Services.AddSingleton<IDocuementContentStore, S3Store>();
		builder.Services.AddSingleton<IPreviewContentStore, S3Store>();

		return builder;
	}

	public static WebApplicationBuilder AddPublicLinks(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IShareService, ShareService>();

		return builder;
	}

	public static WebApplicationBuilder AddPreviewGeneration(this WebApplicationBuilder builder)
	{
		builder.Services.AddSingleton<IPreviewGenerator, DummyPreviewService>();
		builder.Services.AddSingleton<IPreviewViewer, DummyPreviewService>();

		return builder;
	}

	public static WebApplicationBuilder AddDocumentsStore(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<UploadValidatorSettings>(
			builder.Configuration.GetSection(UploadValidatorSettings.Section));

		builder.Services.AddSingleton<IUploadValidator, UploadValidator>();
		builder.Services.AddScoped<IDocumentStorage, DocumentStorage>();
		builder.Services.AddScoped<IMetadataStorage, DocumentStorage>();
		builder.Services.AddScoped<IZipper, DocumentStorage>();

		return builder;
	}
}
