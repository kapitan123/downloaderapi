using DocumentStore.Domain.Preview;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Previews;

// AK TODO add versioning
[ApiController]
[Route("api")]
public class PreviewsController(IPreviewViewer previewViewer, ILogger<PreviewsController> logger) : ControllerBase
{
	[HttpPost("documents/{id}/preview", Name = "GetPreview")]
	[Produces("image/jpeg")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAll([FromRoute] Guid id, CancellationToken token)
	{
		try
		{
			var (content, size) = await previewViewer.ViewForDocument(id, token);
			Response.ContentLength = size;
			Response.Headers.Append("Accept-Ranges", "bytes");
			Response.Headers.Append("Content-Range", "bytes 0-" + size);
			return File(content, "image/jpeg", $"{id}-preview");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error occurred during fetching a preview of a document {id}", id);
			return StatusCode(500, "Internal Server Error");
		}
	}
}
