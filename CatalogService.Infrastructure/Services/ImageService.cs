using CatalogService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _config;
        public ImageService(IConfiguration config) => _config = config;
        public string GetImageUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            return $"{_config["AppSettings:BaseUrl"]}/uploads/{fileName}";
        }
    }
}
