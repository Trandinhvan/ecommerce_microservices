using Microsoft.EntityFrameworkCore;
using OrderingService.Application.Interfaces;
using OrderingService.Infrastructure.Data;
using OrderingService.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderingService.Application.UseCase;
using OrderingService.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Thêm Logging
builder.Services.AddLogging();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Frontend của bạn
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Đăng ký RabbitMqMessageBus vào DI
builder.Services.AddSingleton<IMessageBus>(sp =>
{
    return RabbitMqMessageBus.CreateAsync().GetAwaiter().GetResult();
});

// DbContext
// DbContext MẶC ĐỊNH LÀ Scoped
builder.Services.AddDbContext<OrderingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository + UseCase (Scoped)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderUseCase, OrderUseCase>();

// 🎯 Đăng ký Hosted Service cho Consumer 
// Đây là cách CHUẨN để Consumer chạy nền liên tục.
builder.Services.AddHostedService<PaymentResultBackgroundService>();

// Authentication + Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ❌ Xóa đoạn code cũ khởi tạo consumer thủ công ở Program.cs

app.Run();

//---------------------------------------------------------
// HOSTED SERVICE MỚI
//---------------------------------------------------------

// Dùng để chạy PaymentResultConsumer trong nền (background)
public class PaymentResultBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentResultBackgroundService> _logger;
    private PaymentResultConsumer? _consumer;

    public PaymentResultBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PaymentResultBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting PaymentResult Consumer Service.");

        // Tạo scope riêng để lấy các service cần thiết (ScopeFactory và Logger là Singleton, có thể inject thẳng)
        // Lưu ý: Không cần tạo scope riêng ở đây vì PaymentResultConsumer đã tự xử lý scope bên trong ReceivedAsync.
        try
        {
            // Cần lấy logger cho consumer bên trong scope để logger này có thể được inject vào consumer
            var consumerLogger = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<PaymentResultConsumer>>();

            _consumer = await PaymentResultConsumer.CreateAsync(_scopeFactory, consumerLogger);
            await _consumer.StartAsync();

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start PaymentResultConsumer.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping PaymentResult Consumer Service.");
        if (_consumer != null)
        {
            await _consumer.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}
