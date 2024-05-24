using DocumentStore.Controllers;
using DocumentStore.Domain.DocumentUploader;
using DocumentStore.Domain.MimeTypesValidator;
using Microsoft.AspNetCore.Mvc;
using TestSolution.Domain;

namespace TestSolution.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentsController(IDocumentUploader uploader, IContentTypeValidator mimeValidator, ILogger<DocumentsController> logger) : ControllerBase
	{
		private readonly ILogger<DocumentsController> _logger = logger;
		private readonly IDocumentUploader _uploader = uploader;
		private readonly IContentTypeValidator _mimeValidator = mimeValidator;

		// AK TODO add versions to jsons
		[HttpPost("upload", Name = "Upload")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest documentReq, CancellationToken token)
		{
			var file = documentReq.File;
			if (file == null)
			{
				return BadRequest("No file provided.");
			}

			if (file.Length == 0)
			{
				return BadRequest("File must not be empty.");
			}

			if (!_mimeValidator.IsSupported(file.ContentType))
			{
				// AK TODO add rpoblem details
				return BadRequest($"Contenttype {file.ContentType} is not supported. Supported types are: {string.Join(" ,", _mimeValidator.SupportedTypes)}");
			}
			// AK TOFO generate preview in parallel

			// AK TODO should validate supported documentTypes
			try
			{

				var meta = new DocumentMeta
				{
					Name = file.FileName,
					UploadedOn = DateTime.UtcNow,
					Type = file.ContentType,

				};

				// Extract meta
				// Start stream
				Task CopyToAsync(Stream target, CancellationToken cancellationToken = default);
				var filePath = Path.Combine("Uploads", file.FileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream, token);
				}

				// Simulate storing file metadata in your Filestorage
				await _fsclient.StoreFileMetadataAsync(file.FileName, filePath, token);

				return CreatedAtRoute("Upload", new { fileName = file.FileName }, null);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during the file upload.");
				return StatusCode(500, "Internal Server Error");
			}
		}

		// AK TODO add versions to jsons
		[HttpGet("", Name = "GetAll")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]

		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAll(CancellationToken token)
		{

		}

		[HttpGet("{id}")]
		public async Task<IActionResult> DownLoadFile(string id)
		{
			var (content, details) = await _storageService.DownloadFileAsync(id);
			this.Response.ContentLength = details.Size;
			this.Response.Headers.Add("Accept-Ranges", "bytes");
			this.Response.Headers.Add("Content-Range", "bytes 0-" + details.Size);
			return File(content, details.ContentType, details.Name);
		}

	}
}
