using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrderingService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using OrderingService.Contracts.Events;
using OrderingService.Domain.Enums;
using Microsoft.Extensions.Logging; // Thêm logger để debug

namespace OrderingService.Infrastructure.Messaging
{
    // Loại bỏ IOrderUseCase khỏi constructor
    public class PaymentResultConsumer : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentResultConsumer> _logger;
        private readonly string _queueName = "ordering_payment_results";
        private bool _disposed;

        // Constructor chỉ nhận Connection, Channel, ScopeFactory và Logger
        private PaymentResultConsumer(
            IConnection connection,
            IChannel channel,
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentResultConsumer> logger)
        {
            _connection = connection;
            _channel = channel;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Factory async tạo consumer
        // Loại bỏ IOrderUseCase khỏi tham số
        public static async Task<PaymentResultConsumer> CreateAsync(IServiceScopeFactory scopeFactory, ILogger<PaymentResultConsumer> logger)
        {
            // Thiết lập Factory
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            // Tạo kết nối và channel
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            // Khai báo và Bind Queue
            await channel.ExchangeDeclareAsync(exchange: "payment_exchange", type: "direct", durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(queue: "ordering_payment_results", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: "ordering_payment_results", exchange: "payment_exchange", routingKey: "payment.succeeded");
            await channel.QueueBindAsync(queue: "ordering_payment_results", exchange: "payment_exchange", routingKey: "payment.failed");

            return new PaymentResultConsumer(connection, channel, scopeFactory, logger);
        }

        // Bắt đầu nhận message từ queue
        public async Task StartAsync()
        {
            // Cài đặt Quality of Service (QoS) để tránh Consumer bị quá tải
            // Chỉ gửi 1 tin nhắn Unacked tại một thời điểm cho Consumer này
            await _channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Khi nhận được message
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                // TẠO MỘT SCOPE MỚI CHO MỖI TIN NHẮN
                // Điều này đảm bảo IOrderUseCase và DbContext là mới và được dispose đúng cách
                using (var scope = _scopeFactory.CreateScope())
                {
                    // LẤY IOrderUseCase BÊN TRONG SCOPE
                    var orderUseCase = scope.ServiceProvider.GetRequiredService<IOrderUseCase>();

                    try
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                        if (ea.RoutingKey == "payment.succeeded")
                        {
                            var evt = JsonSerializer.Deserialize<PaymentSucceeded>(body);
                            _logger.LogInformation($"[OrderingService] Payment succeeded: OrderId={evt!.OrderId}");

                            // Sử dụng orderUseCase vừa được lấy từ Scope
                            await orderUseCase.UpdateOrderStatusAsync(evt.OrderId, OrderStatus.Paid);
                        }
                        else if (ea.RoutingKey == "payment.failed")
                        {
                            var evt = JsonSerializer.Deserialize<PaymentFailed>(body);
                            _logger.LogInformation($"[OrderingService] Payment failed: OrderId={evt!.OrderId}");

                            // Sử dụng orderUseCase vừa được lấy từ Scope
                            await orderUseCase.UpdateOrderStatusAsync(evt.OrderId, OrderStatus.Cancelled);
                        }

                        // Xác nhận message đã được xử lý thành công
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"[OrderingService] Error processing payment result for DeliveryTag {ea.DeliveryTag}: {ex.Message}");

                        // Trả message lại queue để retry (requeue: true)
                        // Bạn có thể cân nhắc gửi đến Dead Letter Exchange nếu không muốn retry ngay lập tức
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                } // Khi thoát khỏi 'using', scope và DbContext bên trong sẽ được dispose an toàn.
            };

            // Bắt đầu consume
            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Consumer started, listening on queue: {_queueName}");
        }

        // Dispose connection và channel
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
                _logger.LogError(ex, "[PaymentResultConsumer] Dispose error.");
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
