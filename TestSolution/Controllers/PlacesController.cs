using Microsoft.AspNetCore.Mvc;
using TestSolution.Domain;
using TestSolution.Infrastructrue.Web;

namespace TestSolution.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PlacesController(IForsquareSearchPlacesClient client, ILogger<PlacesController> logger) : ControllerBase
	{
		private readonly ILogger<PlacesController> _logger = logger;
		private readonly IForsquareSearchPlacesClient _fsclient = client;

		[HttpGet(Name = "Place")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Get(CancellationToken token)
		{
			try
			{
				var geo = new MainGeocode(41.878, -87.6298);
				var fetchResults = await _fsclient.SearchInARadiusOf(geo, 1000, token);

				var response = new GetSearchResult
				{
					Data = fetchResults.Select(x => new PlaceResponse { Name = x.Name, MainGeocode = x.MainGeocode }).ToList(),
				};

				return Ok(response);
			}

			catch (Exception ex)
			{
				return StatusCode(500, "Internal Server Error");
			}
		}
	}
}
