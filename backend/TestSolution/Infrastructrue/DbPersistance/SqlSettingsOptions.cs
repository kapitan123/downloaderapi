namespace DocumentStore.Infrastructrue.DbPersistance;

public class SqlSettingsOptions
{
	public static string Section => "Sql";
	public string ConnectionString { get; set; } = "";
}