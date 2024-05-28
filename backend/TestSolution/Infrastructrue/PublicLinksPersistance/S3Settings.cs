namespace DocumentStore.Infrastructrue.FileSystem;

public class RedisSettings
{
	public static string Section => "RedisSettings";

	public string ConnectionString { get; set; }
}
