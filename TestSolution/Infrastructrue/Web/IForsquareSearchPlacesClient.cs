using TestSolution.Domain;

namespace TestSolution.Infrastructrue.Web;
public interface IForsquareSearchPlacesClient
{
	Task<List<Place>> SearchInARadiusOf(MainGeocode point, int radiusInMeters, CancellationToken token);
}