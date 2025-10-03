using BasketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketService.Infrastructure.Data
{
    public class BasketDBContext : DbContext
    {
        public BasketDBContext(DbContextOptions<BasketDBContext> options) : base(options) { }
        public DbSet<Basket> Baskets => Set<Basket>();
    }
}
