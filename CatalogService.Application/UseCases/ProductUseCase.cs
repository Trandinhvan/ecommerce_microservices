using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.UseCases
{
    public class ProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;
        private readonly IFileStorageService _fileStorageService;

        public ProductUseCase(IProductRepository productRepository, IImageService imageService, IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _imageService = imageService;
            _fileStorageService = fileStorageService;
        }

        // Lấy tất cả sản phẩm cho UI
        public async Task<IEnumerable<ProductDisplayDto>> GetAllProductsForDisplayAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(p => new ProductDisplayDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Category?.Name ?? string.Empty, // lấy CategoryName
                _imageService.GetImageUrl(p.ImageUrl),
                p.CreatedAt
            ));
        }

        //public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        //{
        //    var products = await _productRepository.GetAllAsync();
        //    return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.CategoryId));
        //}

        public async Task<ProductDetailDto> GetProductDetailById(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null) return null;

            return new ProductDetailDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.OriginalPrice,
                product.StockQuantity,
                _imageService.GetImageUrl(product.ImageUrl),
                product.Category?.Name ?? string.Empty,
                product.CreatedAt,
                product.UpdatedAt
            );
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.CategoryId);
        }

        //public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
        //{
        //    var product = new Product
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = request.Name,
        //        Description = request.Description ?? string.Empty,
        //        Price = request.Price,
        //        CategoryId = request.CategoryId,
        //        ImageUrl = request.ImageUrl ?? string.Empty
        //    };
        //    // Logic nghiệp vụ: Ví dụ, kiểm tra ràng buộc nghiệp vụ trước khi lưu
        //    await _productRepository.AddAsync(product);
        //    return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.CategoryId);
        //}
        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, Stream? fileStream = null, string? fileName = null)
        {
            string? imageUrl = null;
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                imageUrl = await _fileStorageService.SaveFileAsync(fileStream, fileName);
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                ImageUrl = imageUrl
            };

            await _productRepository.AddAsync(product);

            return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.CategoryId);
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                // Thay vì trả về NotFound, Use Case sẽ ném ra ngoại lệ
                // để Controller xử lý.
                throw new InvalidOperationException("Product not found.");
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return;
            // Xóa file nếu có
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var fileName = Path.GetFileName(product.ImageUrl); // lấy tên file từ URL
                await _fileStorageService.DeleteFileAsync(fileName);
            }
            await _productRepository.DeleteAsync(id);
        }

    }
}
