using DocumentStore.Domain.DocumentUploader;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Previews;

// AK TODO add versioning
[ApiController]
[Route("api")]
public class PreviewsController(IDocumentStorage docStore, ILogger<PreviewsController> logger) : ControllerBase
{
	[HttpPost("documents/{id}/preview", Name = "GetPreview")]
	[Produces("image/jpeg")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAll(Guid id, CancellationToken token)
	{
		try
		{
			throw new NotImplementedException();
			return Ok();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error occurred during fetching all documents meta.");
			return StatusCode(500, "Internal Server Error");
		}
	}
}
