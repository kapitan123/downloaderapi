namespace DocumentStore.Infrastructrue.MetadataPersistance;

public class SqlSettingsOptions
{
	public static string Section => "TestHttpClient";
	public string Url { get; set; } = "";
	public string ApiKey { get; set; } = "";
}