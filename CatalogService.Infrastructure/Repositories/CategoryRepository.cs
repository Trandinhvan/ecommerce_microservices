using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogDbContext _dbcontext;

        public CategoryRepository(CatalogDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Category?> GetByIdAsync(Guid id) 
            => await _dbcontext.Categories.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _dbcontext.Categories.ToListAsync();

        public async Task AddAsync(Category category)
        {
            _dbcontext.Categories.Add(category);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _dbcontext.Categories.Update(category);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if(category != null)
            {
                _dbcontext.Remove(category);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}
