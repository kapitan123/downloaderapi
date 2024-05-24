using Microsoft.AspNetCore.Mvc;
using TestSolution.Infrastructrue.Persistance;
using TestSolution.Infrastructrue.Web;

namespace TestSolution.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentsController(FileContentStore fileStore, IDocumentsMetadataRepository metaRepo, ILogger<DocumentsController> logger) : ControllerBase
	{
		private readonly ILogger<DocumentsController> _logger = logger;
		private readonly FileContentStore _fileStore = fileStore;
		private readonly IDocumentsMetadataRepository _metaRepo = metaRepo;

		// AK TODO add versions to jsons
		[HttpPost("upload", Name = "Upload")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Upload(IFormFile file, CancellationToken token)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file provided or file is empty.");
			}

			try
			{
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
	}
}
