using DocumentStore.Controllers.Errors;
using DocumentStore.Domain.DocumentUploader;
using DocumentStore.Domain.MimeTypesValidator;
using DocumentStore.Domain.PreviewGenerator;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Documents
{
	// AK TODO add versioning
	[ApiController]
	[Route("api/documents")]
	public class DocumentsController(IDocumentStorage store,
		IZipper zipper,
		IPreviewGenerator previewGenerator,
		IContentTypeValidator contentTypeValidator,
		ILogger<DocumentsController> logger) : ControllerBase
	{

		// AK TODO add versions to jsons
		[HttpPost("upload", Name = "Upload")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest req, CancellationToken token)
		{
			var file = req.File;
			if (file == null)
			{
				return BadRequest(ApiErrors.FileNotProvided);
			}

			if (file.Length == 0)
			{
				return BadRequest(ApiErrors.FileMustNotBeEmpty);
			}

			if (!contentTypeValidator.IsSupported(file.ContentType))
			{
				// AK TODO add rpoblem details
				return BadRequest($"Contenttype {file.ContentType} is not supported. " +
					$"Supported types are: {string.Join(" ,", contentTypeValidator.SupportedTypes)}");
			}
			try
			{
				var meta = req.ToMetaData();

				await store.SaveAsync(meta, file.OpenReadStream(), token);

				return Created();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred during the file upload.");
				return StatusCode(500, "Internal Server Error");
			}
		}

		[HttpGet("{id}", Name = "Download")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownLoadFile(Guid id, CancellationToken token)
		{
			var (meta, content) = await store.GetAsync(id, token);
			Response.ContentLength = meta.Size;
			Response.Headers.Append("Accept-Ranges", "bytes");
			Response.Headers.Append("Content-Range", "bytes 0-" + meta.Size);
			return File(content, meta.ContentType, meta.Name);
		}

		// AK TODO add versions to jsons should go to a metadata controller
		[HttpGet("download-zip", Name = "DownloadZip")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownloadZip(List<Guid> ids, CancellationToken token)
		{
			if (ids?.Count < 1)
			{
				return BadRequest("No document Ids were provided");
			}

			if (ids.Count > 10)
			{
				return BadRequest("The max number of zipped files is 10");
			}

			var zipName = $"{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.zip";

			var zipStream = await zipper.GetZipedFiles(ids, token);

			return File(zipStream, "application/zip", zipName);
		}
	}
}
