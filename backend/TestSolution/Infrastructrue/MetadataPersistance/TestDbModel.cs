using System.ComponentModel.DataAnnotations;

namespace DocumentStore.Infrastructrue.MetadataPersistance
{
    public class TestDbModel
    {
        [Key]
        public Guid Id { get; set; }
    }
}
