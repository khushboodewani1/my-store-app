using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Model
{
    public class CategoryMaster
    {
        public int Id { get; set; }
        public string Name { get; set; } 

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
