using CatalogService.Application.Interfaces;
using CatalogService.Application.UseCases;
using CatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductUseCase _productUseCase;
        private readonly IFileStorageService _fileStorageService;

        public ProductController(ProductUseCase productUseCase, IFileStorageService fileStorageService)
        {
            _productUseCase = productUseCase;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var products = await _productUseCase.GetAllProductsAsync();
            
            var products = await _productUseCase.GetAllProductsForDisplayAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //var product = await _productUseCase.GetProductByIdAsync(id);
            var product = await _productUseCase.GetProductDetailById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateProductRequest request)
        //{
        //    var createdProduct = await _productUseCase.CreateProductAsync(request);
        //    return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] CreateProductRequest request, IFormFile? file)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        string? filePath = null;
        //        if (file != null && file.Length > 0)
        //        {
        //            // gọi sang service lưu file
        //            filePath = await _fileStorageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
        //        }

        //        var productRequest = new CreateProductRequest
        //        {
        //            Name = request.Name,
        //            Description = request.Description,
        //            Price = request.Price,
        //            CategoryId = request.CategoryId,
        //            ImageUrl = filePath
        //        };

        //        var createdProduct = await _productUseCase.CreateProductAsync(productRequest);

        //        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi khi tạo sản phẩm: {ex.Message}");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request, IFormFile? file)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Stream? fileStream = file?.OpenReadStream();
                string? fileName = file?.FileName;

                var createdProduct = await _productUseCase.CreateProductAsync(request, fileStream, fileName);

                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
        {
           try
           {
                await _productUseCase.UpdateProductAsync(id,request);
                return NoContent();
           } 
           catch (InvalidOperationException)
           {
                return NotFound();
           }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productUseCase.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
