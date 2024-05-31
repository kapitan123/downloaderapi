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
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.ServiceCollectionExtensions;

// I put serviceExtensions in the api layer
// Because I grouped code by layers
// Though generally I prefer grouping code by features, because this way you can trully chive a pluggable architecture
public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddInfra(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<SqlSettingsOptions>(
			builder.Configuration.GetSection(SqlSettingsOptions.Section));

		builder.Services.Configure<SqlSettingsOptions>(
			builder.Configuration.GetSection(SqlSettingsOptions.Section));

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

		builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();

		builder.Services.AddSingleton<ITransferUtility, TransferUtility>();
		builder.Services.AddSingleton<IDocuementContentStore, S3Store>();

		return builder;
	}

	public static WebApplicationBuilder AddPublicLinks(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IPublicLinkRepository, PublicLinkRepository>();
		builder.Services.AddSingleton<IShareService, ShareService>();

		return builder;
	}

	public static WebApplicationBuilder AddPreviewGeneration(this WebApplicationBuilder builder)
	{
		builder.Services.AddSingleton<IPreviewGenerator, DummyPreviewService>();
		builder.Services.AddSingleton<IPreviewViewer, DummyPreviewService>();
		builder.Services.AddSingleton<IPreviewContentStore, S3Store>();

		return builder;
	}

	public static WebApplicationBuilder AddDocumentsStore(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<UploadValidatorSettings>(
			builder.Configuration.GetSection(UploadValidatorSettings.Section));

		builder.Services.AddSingleton<IUploadValidator, UploadValidator>();
		builder.Services.AddSingleton<IDocumentStorage, DocumentStorage>();
		builder.Services.AddSingleton<IMetadataStorage, DocumentStorage>();
		builder.Services.AddSingleton<IZipper, DocumentStorage>();

		return builder;
	}
}
