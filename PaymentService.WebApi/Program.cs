using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Application.UseCase;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddScoped<IPaymentUseCase, PaymentUseCase>();


builder.Services.AddSingleton<IPaymentPublisher>(sp =>
{
    return PaymentPublisher.CreateAsync().GetAwaiter().GetResult();
});


// Singleton nhưng truyền IServiceScopeFactory
builder.Services.AddSingleton<OrderCreatedConsumer>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return OrderCreatedConsumer.CreateAsync(scopeFactory).GetAwaiter().GetResult();
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Start RabbitMQ consummer
var consumer = app.Services.GetRequiredService<OrderCreatedConsumer>();
await consumer.StartAsync();

app.Run();
