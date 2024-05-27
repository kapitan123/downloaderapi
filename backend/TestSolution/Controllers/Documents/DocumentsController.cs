using DocumentStore.Controllers.Errors;
using DocumentStore.Domain.Documents;
using DocumentStore.Domain.ShareabaleUrls;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Documents
{
	// AK TODO add versioning
	[ApiController]
	[Route("api/documents")]
	public class DocumentsController(IDocumentStorage store,
		IShareService shareService,
		IZipper zipper,
		IUploadValidator uploadValidator,
		ILogger<DocumentsController> logger) : ControllerBase
	{
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
			// AK TODOthis can be moved to validator itself
			if (file.Length > uploadValidator.MaxSize)
			{
				return BadRequest("file is too big");
			}

			if (!uploadValidator.IsSupported(file.ContentType))
			{
				// AK TODO add rpoblem details
				return BadRequest($"Contenttype {file.ContentType} is not supported. " +
					$"Supported types are: {string.Join(" ,", uploadValidator.SupportedTypes)}");
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

		[HttpGet("{id}/share", Name = "Share")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetShareUrl([FromRoute] Guid id, int expiration, CancellationToken token)
		{
			// It would be better to use an inbuilt s3 presigned url,
			// But we would not be able to count how many times the file was downloaded,
			// Only how many times we generated a url download a link.
			var uri = await shareService.GetPublicUriFor(id, expiration);
			return Ok(uri);
		}

		[HttpGet("shared/{id}/download", Name = "DownloadShared")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status410Gone)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownloadFromShareUrl([FromRoute] Guid pudlicId, int expiration, CancellationToken token)
		{
			// It would be better to use an inbuilt s3 presigned url,
			// But we would not be able to count how many times the file was downloaded,
			// Only how many times we generated a url download a link.
			var uri = await shareService.GetDocumentByPublicId(pudlicId);
			return Ok(uri);
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

			var zipStream = await zipper.GetZipedFiles(ids, token);

			return File(zipStream, "application/zip", $"{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.zip");
		}
	}
}
