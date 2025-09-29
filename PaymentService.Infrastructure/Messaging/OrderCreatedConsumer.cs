using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Contracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentService.Infrastructure.Messaging
{
    public class OrderCreatedConsumer : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _exchangeName = "ordering_exchange";
        private readonly string _queueName = "payment_order_created";
        private bool _disposed;

        private OrderCreatedConsumer(IConnection connection, IChannel channel, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _channel = channel;
            _scopeFactory = scopeFactory;
        }

        public static async Task<OrderCreatedConsumer> CreateAsync(IServiceScopeFactory scopeFactory)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            // ✅ Khai báo exchange nếu chưa tồn tại
            await channel.ExchangeDeclareAsync(
                exchange: "ordering_exchange",
                type: ExchangeType.Direct,
                durable: true
            );

            // ✅ Khai báo queue
            await channel.QueueDeclareAsync(
                queue: "payment_order_created",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // ✅ Bind queue vào exchange
            await channel.QueueBindAsync(
                queue: "payment_order_created",
                exchange: "ordering_exchange",
                routingKey: "order.created"
            );

            return new OrderCreatedConsumer(connection, channel, scopeFactory);
        }


        public async Task StartAsync()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var order = JsonSerializer.Deserialize<OrderCreated>(json);

                    Console.WriteLine($"[PaymentService] Received OrderCreated: {order!.OrderId}");

                    // 👉 Tạo scope mới mỗi lần xử lý message
                    using var scope = _scopeFactory.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<IPaymentUseCase>();

                    await useCase.HandlePaymentAsync(new CreatePaymentRequest(
                        order.OrderId, order.UserId.ToString(), order.TotalAmount, "MoMo"
                    ));

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PaymentService] Error handling message: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            try
            {
                if (_channel != null) await _channel.CloseAsync();
                if (_connection != null) await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderCreatedConsumer] Dispose error: {ex.Message}");
            }
            finally
            {
                if (_channel != null) await _channel.DisposeAsync();
                if (_connection != null) await _connection.DisposeAsync();
                _disposed = true;
            }
        }
    }
}
