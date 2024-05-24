using TestSolution.Domain;

namespace TestSolution.Controllers;

public class GetSearchResult
{
	public List<PlaceResponse> Data { get; set; }
}


public class PlaceResponse
{
	public string Name { get; set; }

	public MainGeocode MainGeocode { get; set; }
}