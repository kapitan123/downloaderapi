namespace TestSolution.Infrastructrue.Web;

public class ForsquareHttpClientOptions
{
	public static string Section => "TestHttpClient";
	public string Url { get; set; } = "";
	public string ApiKey { get; set; } = "";
}
