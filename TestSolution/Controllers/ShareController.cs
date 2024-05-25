using Microsoft.AspNetCore.Mvc;
using TestSolution.Infrastructrue.Persistance;

namespace TestSolution.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ShareController(IMetadataRepository metaRepo, ILogger<ShareController> logger) : ControllerBase
	{
		private readonly ILogger<ShareController> _logger = logger;
		private readonly IMetadataRepository _metaRepo = metaRepo;

		// AK TODO add versions to jsons
		// AK TODO I'm not a big fan of a noun based apis
		[HttpGet("share", Name = "share")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Get(IFormFile file, CancellationToken token)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file provided or file is empty.");
			}

			try
			{

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
	}
}
