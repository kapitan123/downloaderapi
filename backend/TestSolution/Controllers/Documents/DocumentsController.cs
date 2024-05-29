using Asp.Versioning;
using DocumentStore.Controllers.Documents.Validation;
using DocumentStore.Controllers.Errors;
using DocumentStore.Domain.Documents;
using DocumentStore.Domain.ShareabaleUrls;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Documents
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/documents")]
	// I prefer not use excessive abstraction/inderection like mediatR or UseCases/CQRS when the benefit is not clear.
	// And work directly with domain services.
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

			var result = uploadValidator.Validate(file);

			if (result.Value is ApiError err)
			{
				return BadRequest(err);
			}

			var meta = req.ToMetaData();

			await store.SaveAsync(meta, file.OpenReadStream(), token);

			return Created();

		}

		[HttpGet("{id}/share", Name = "Share")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetShareUrl([FromRoute] Guid id, int expirationInHours, CancellationToken token)
		{
			var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

			// It would be better to use an inbuilt s3 presigned url,
			// But we would not be able to count how many times the file was downloaded,
			// Only how many times we generated a url download a link.
			// Also we assume there is basically no limit for expiration hours
			var publicId = await shareService.GenerateTempPublicIdFor(id, expirationInHours, token);

			var resp = new ShareDocumentResponse()
			{
				Data = new Uri($"{location.AbsoluteUri}/shared/{publicId}/download")
			};

			return Ok(resp);
		}

		// Would be nice to have a Status410Gone for expired links
		// But it will require custom expiration service 
		[HttpGet("shared/{pudlicId}/download", Name = "DownloadShared")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
				async notFound => NotFound());
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

		// DownloadZip does not have an error handlung branch for missing documents
		// In this case we treat any missing file as a complete failure
		[HttpGet("download-zip", Name = "DownloadZip")]
		[Produces("application/octet-stream")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DownloadZip(List<Guid> ids, CancellationToken token)
		{
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
				var zipStream = await zipper.GetZipedFiles(ids, token);

				return File(zipStream, "application/zip", $"{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.zip");
			}
			catch (KeyNotFoundException)
			{
				return BadRequest();
			}
		}
	}
}
