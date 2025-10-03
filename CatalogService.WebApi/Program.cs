using CatalogService.Application.Interfaces;
using CatalogService.Application.UseCases;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using CatalogService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Frontend của bạn
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // nếu bạn dùng cookie hoặc credential
    });
});

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductUseCase>();// Đăng ký use case

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped < CategoryUseCase>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IImageService, ImageService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Serve static files from App_Data/uploads
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "App_Data/uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); tắt mới chạy được http

app.UseAuthorization();

app.MapControllers();

app.Run();
