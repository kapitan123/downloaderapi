using Amazon.S3.Transfer;
using DocumentStore.Controllers.Documents.Validation;
using DocumentStore.Domain.Documents;
using DocumentStore.Domain.Preview;
using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Infrastructrue.DbPersistance;
using DocumentStore.Infrastructrue.FileSystem;
using DocumentStore.Infrastructrue.MetadataPersistance;
using TestSolution.Infrastructrue.Web;

namespace DocumentStore.ServiceCollectionExtensions;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddDb(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<SqlSettingsOptions>(
		builder.Configuration.GetSection(SqlSettingsOptions.Section));

		builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
		builder.Services.AddScoped<IPublicLinkRepository, PublicLinkRepository>();

		return builder;
	}

	public static WebApplicationBuilder AddS3(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<S3Settings>(
			builder.Configuration.GetSection(S3Settings.Section));

		builder.Services.AddSingleton<ITransferUtility, TransferUtility>();
		builder.Services.AddSingleton<IPreviewContentStore, S3Store>();
		builder.Services.AddSingleton<IDocuementContentStore, S3Store>();

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
		builder.Services.AddSingleton<IDocumentStorage, DocumentStorage>();
		builder.Services.AddSingleton<IMetadataStorage, DocumentStorage>();
		builder.Services.AddSingleton<IZipper, DocumentStorage>();

		return builder;
	}
}
