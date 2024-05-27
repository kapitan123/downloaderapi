using DocumentStore.Domain.DocumentUploader;
using Microsoft.AspNetCore.Mvc;

namespace TestSolution.Controllers
{
	// AK TODO add versioning
	[ApiController]
	[Route("api")]
	public class DocumentsMetaController(IMetadataStorage docStore, ILogger<DocumentsMetaController> logger) : ControllerBase
	{

		[HttpPost("documents/details", Name = "GetAllMeta")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAll(CancellationToken token)
		{
			try
			{
				var result = docStore.GetMetaOfAllDocuments();
				return Ok();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred during fetching all documents meta.");
				return StatusCode(500, "Internal Server Error");
			}
		}
	}
}
