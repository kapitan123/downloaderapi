using Asp.Versioning;
using DocumentStore.Controllers.DocumentsMeta;
using DocumentStore.Domain.Documents;
using Microsoft.AspNetCore.Mvc;

namespace TestSolution.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("apiapi/v{version:apiVersion}/documents")]
	public class DocumentsMetaController(IMetadataStorage docStore, ILogger<DocumentsMetaController> logger) : ControllerBase
	{
		// Production version should have a coursor paging
		[HttpPost("metadata", Name = "GetAllMeta")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAll(CancellationToken token)
		{
			var result = await docStore.GetMetaOfAllDocumentsAsync(token);
			var resp = new GetDocumentsMetadataResult
			{
				Data = result
			};

			return Ok(resp);
		}
	}
}
