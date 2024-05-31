namespace DocumentStore.Infrastructrue.DbPersistance;

public class SqlSettings
{
	public static string Section => "Sql";
	public string ConnectionString { get; set; } = "";
}