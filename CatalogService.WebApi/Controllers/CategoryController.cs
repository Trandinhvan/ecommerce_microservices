using CatalogService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryUseCase _categoryUseCase;
        public CategoryController(CategoryUseCase categoryUseCase)
        {
            _categoryUseCase = categoryUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           var categories = await _categoryUseCase.GetAllCategory();
            return Ok(categories);
        }

    }
}
