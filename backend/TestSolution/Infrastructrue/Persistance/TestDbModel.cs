using System.ComponentModel.DataAnnotations;

namespace TestSolution.Infrastructrue.Persistance
{
	public class TestDbModel
	{
		[Key]
		public Guid Id { get; set; }
	}
}
