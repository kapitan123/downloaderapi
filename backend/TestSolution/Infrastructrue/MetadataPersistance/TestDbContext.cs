using Microsoft.EntityFrameworkCore;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public class TestDbContext : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseInMemoryDatabase(databaseName: "TestDb");
	}

	public DbSet<TestDbModel> TestItems { get; set; }
}
