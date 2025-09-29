using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.UseCases
{
    public class CategoryUseCase
    {
        private readonly ICategoryRepository _repo;

        public CategoryUseCase(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if(category == null)
            {
                return null;
            }
            return new CategoryDto(category.Id, category.Name);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategory()
        {
            var categories = await _repo.GetAllAsync();
            return categories.Select(c => new CategoryDto(c.Id, c.Name));
        }
    }
}
