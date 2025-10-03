using CatalogService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _uploadPath;

        public LocalFileStorageService(IConfiguration config)
        {
            _uploadPath = config["FileStorage:UploadPath"]
                          ?? Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Uploads");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            // Trả về relative path để lưu trong DB
            return uniqueFileName;
        }

        public Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return Task.CompletedTask;

            var filePath = Path.Combine(_uploadPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
