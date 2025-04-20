using System.ComponentModel.DataAnnotations;

namespace MyStore.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public Price Price { get; set; } = new Price();

        [Required]
        public int CategoryMasterId { get; set; }
        public virtual CategoryMaster? CategoryMaster { get; set; }
    }
}
