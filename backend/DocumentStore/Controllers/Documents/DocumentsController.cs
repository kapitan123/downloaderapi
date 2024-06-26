using Asp.Versioning;
using DocumentStore.Controllers.Documents.Validation;
using DocumentStore.Domain.Documents;
using DocumentStore.Domain.ShareabaleUrls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DocumentStore.Controllers.Documents
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/documents")]
	// I prefer not use excessive abstraction/inderection like mediatR or UseCases/CQRS when the benefit is not clear.
	// And work directly with domain services as long as it's convinient and clear
	public class DocumentsController(IDocumentStorage store,
		IShareService shareService,
		IZipper zipper,
		IUploadValidator uploadValidator,
		ILogger<DocumentsController> logger) : ControllerBase
	{
		[HttpPost("upload", Name = "Upload")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest req, CancellationToken token)
		{
			var file = req.File;

			var result = uploadValidator.Validate(file);

			if (result.Value is ApiError err)
			{
				return BadRequest(err);
			}

			var meta = req.ToMetaData();

			var newDocId = await store.SaveAsync(meta, file.OpenReadStream(), token);

			return Ok(new DocumentCreatedResult { Id = newDocId });
		}

		[HttpGet("{id}", Name = "Download")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownLoadFile(Guid id, CancellationToken token)
		{
			// This method uses one of conventional ways to handle NotFound cases
			try
			{
				var (meta, content) = await store.GetAsync(id, token);
				Response.ContentLength = meta.Size;
				Response.Headers.Append("Accept-Ranges", "bytes");
				Response.Headers.Append("Content-Range", "bytes 0-" + meta.Size);
				return File(content, meta.ContentType, meta.Name);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpGet(Name = "DownloadUserFile")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownUserFile(Guid id, [BindRequired] string user, CancellationToken token)
		{
			// This method uses one of conventional ways to handle NotFound cases
			try
			{
				// user value should be taken from user Identity
				// I pass it as a query param as not to introduce custom authentication filters
				// It's not an acceptable way to do it in prod
				var (meta, content) = await store.GetFilteredByUserAsync(id, user, token);
				Response.ContentLength = meta.Size;
				Response.Headers.Append("Accept-Ranges", "bytes");
				Response.Headers.Append("Content-Range", "bytes 0-" + meta.Size);
				return File(content, meta.ContentType, meta.Name);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (UnauthorizedAccessException ex)
			{
				logger.LogWarning(message: ex.Message);
				return Forbid();
			}
		}

		[HttpPost("{id}/share", Name = "Share")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateShareUrl([FromRoute] Guid id, [BindRequired] int expirationInHours, CancellationToken token)
		{
			// the method should also check if user has acces to the file
			// and that th file actually exists
			// these parts were skipped
			if (expirationInHours < 1)
			{
				return BadRequest("Minimal link lifetime is 1 hour.");
			}

			// It would be better to use an inbuilt s3 presigned url,
			// But we would not be able to count how many times the file was downloaded,
			// Only how many times we generated a url download a link.
			// Also we assume there is basically no limit for expiration hours
			var publicId = await shareService.GenerateTempPublicIdFor(id, expirationInHours, token);

			var sharedUrl = $"{Request.Scheme}://{Request.Host}/api/v1/documents/shared/{publicId}/download";

			var resp = new ShareDocumentResponse()
			{
				Data = new Uri(sharedUrl)
			};

			return Ok(resp);
		}

		[HttpGet("shared/{pudlicId}/download", Name = "DownloadShared")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status410Gone)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownloadFromShareUrl([FromRoute] string pudlicId, CancellationToken token)
		{
			// It would be better to use an inbuilt s3 presigned url,
			// But we would not be able to count how many times the file was downloaded,
			// Only how many times we generated a url download link.
			var result = await shareService.GetDocumentIdByPublicId(pudlicId, token);

			// This is my preferred way to handle cases like NotFound
			return await result.Match<Task<IActionResult>>(
				async docId => await DownLoadFile(docId, token),
				async expired => StatusCode(StatusCodes.Status410Gone),
				async notFound => NotFound());
		}

		// DownloadZip does not have an error handlung branch for missing documents
		// In this case we treat any missing file as a complete failure
		[HttpGet("download-zip", Name = "DownloadZip")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownloadZip([FromQuery, BindRequired] List<Guid> ids, CancellationToken token)
		{
			// We assume that id list contains no duplicates,
			// we also assume that user has access to any file from the list
			// to not spend to much time on error handling of edge cases
			if (ids?.Count < 2)
			{
				return BadRequest(ZipDownloadError.NotFilesToZip());
			}

			// This could be taken from setting.json
			var maxFiles = 10;

			if (ids.Count > maxFiles)
			{
				return BadRequest(ZipDownloadError.TooManyFilesToZip(maxFiles));
			}
			try
			{
				var zipStream = await zipper.GetZipedFilesAsync(ids, token);

				return File(zipStream, "application/zip", $"{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.zip");
			}
			catch (KeyNotFoundException)
			{
				return BadRequest();
			}
		}
	}
}
