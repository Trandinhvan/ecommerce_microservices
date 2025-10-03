using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal? OriginalPrice { get; set; }

        public int StockQuantity { get; set; } = 0;

        public string ImageUrl { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
