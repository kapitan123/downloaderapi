using System.Text.Json.Serialization;

namespace TestSolution.Infrastructrue.Web;

public class FoursquareResponse
{
	[JsonPropertyName("results")]
	public List<PlaceResp> Results;
}

public class PlaceResp
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("geocodes")]
	public Geocodes Geocodes { get; set; }
}

public class Geocodes
{
	[JsonPropertyName("main")]
	public Main Main { get; set; }
}

public class Main
{
	[JsonPropertyName("latitude")]
	public double Latitude { get; set; }

	[JsonPropertyName("longitude")]
	public double Longitude { get; set; }
}
