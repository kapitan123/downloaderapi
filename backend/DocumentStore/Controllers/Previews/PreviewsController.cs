using Asp.Versioning;
using DocumentStore.Domain.Preview;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStore.Controllers.Previews;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/documents")]
public class PreviewsController(IPreviewViewer previewViewer, ILogger<PreviewsController> logger) : ControllerBase
{
	[HttpPost("{documentId}/preview", Name = "GetPreview")]
	[Produces("image/jpeg")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAll([FromRoute] Guid documentId, CancellationToken token)
	{
		var content = await previewViewer.ViewForDocument(documentId, token);
		return File(content, "image/jpeg", $"{documentId}-preview");
	}
}
