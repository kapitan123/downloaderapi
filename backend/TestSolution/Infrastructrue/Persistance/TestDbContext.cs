using Microsoft.EntityFrameworkCore;

namespace TestSolution.Infrastructrue.Persistance;

public class TestDbContext : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseInMemoryDatabase(databaseName: "TestDb");
	}

	public DbSet<TestDbModel> TestItems { get; set; }
}
