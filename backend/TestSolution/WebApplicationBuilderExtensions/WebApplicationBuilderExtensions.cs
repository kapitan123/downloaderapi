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
	public static WebApplicationBuilder AddPublicLinks(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<SqlSettingsOptions>(
			builder.Configuration.GetSection(SqlSettingsOptions.Section));

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
		builder.Services.Configure<SqlSettingsOptions>(
			builder.Configuration.GetSection(SqlSettingsOptions.Section));

		builder.Services.Configure<UploadValidatorSettings>(
			builder.Configuration.GetSection(UploadValidatorSettings.Section));

		builder.Services.AddSingleton<IUploadValidator, UploadValidator>();
		builder.Services.AddSingleton<IDocumentStorage, DocumentStorage>();
		builder.Services.AddSingleton<IMetadataStorage, DocumentStorage>();
		builder.Services.AddSingleton<IZipper, DocumentStorage>();

		builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();

		builder.Services.AddSingleton<ITransferUtility, TransferUtility>();
		builder.Services.AddSingleton<IDocuementContentStore, S3Store>();

		return builder;
	}
}
