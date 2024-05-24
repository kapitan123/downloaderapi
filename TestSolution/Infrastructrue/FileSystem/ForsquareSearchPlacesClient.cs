using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TestSolution.Domain;

namespace TestSolution.Infrastructrue.Web;

public class ForsquareSearchPlacesClient : IFileContentStore
{
	private readonly HttpClient _httpClient;
	private readonly ForsquareHttpClientOptions _options;

	public ForsquareSearchPlacesClient(HttpClient client, IOptions<ForsquareHttpClientOptions> options)
	{
		_httpClient = client;
		_options = options.Value;

		_httpClient.BaseAddress = new Uri(_options.Url);
		/// _httpClient.DefaultRequestHeaders.Add("Authorization", "fsq38jf5s6BtxsM5GasJ/3pdhr7HlOSL2O6cjpiwegCvd90=");
	}

	public async Task<List<Place>> SearchInARadiusOf(MimeType point, int radius, CancellationToken token)
	{
		var queryStringTemplate = $"?ll={point.Lat}%2C{point.Long}&radius={radius}";

		///var result = await _httpClient.GetAsync(queryStringTemplate, token);

		var request = new HttpRequestMessage(HttpMethod.Get, queryStringTemplate);

		// Set the Authorization header
		request.Headers.Authorization = new AuthenticationHeaderValue("", "fsq38jf5s6BtxsM5GasJ/3pdhr7HlOSL2O6cjpiwegCvd90=");

		// Send the request using HttpClient
		var result = await _httpClient.SendAsync(request, token);

		result.EnsureSuccessStatusCode();

		var payload = await result.Content.ReadAsStringAsync();

		var foursquareResp = JsonSerializer.Deserialize<FoursquareResponse>(payload, options: new JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = true
		});

		var places = foursquareResp!.Results.Select(p => new Place
		{
			Name = p.Name,
			MainGeocode = new MimeType(p.Geocodes.Main.Latitude, p.Geocodes.Main.Longitude)
		}).ToList();

		return places;
	}
}