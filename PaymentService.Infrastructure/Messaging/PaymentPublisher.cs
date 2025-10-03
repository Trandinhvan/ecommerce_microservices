using Microsoft.EntityFrameworkCore.Metadata;
using PaymentService.Application.Interfaces;
using PaymentService.Contracts.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Messaging
{
    public class PaymentPublisher : IAsyncDisposable, IPaymentPublisher
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName = "payment_exchange";
        private bool _disposed;

        private PaymentPublisher(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        // Factory async để tạo instance
        public static async Task<PaymentPublisher> CreateAsync()
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

            // Khai báo exchange (direct)
            await channel.ExchangeDeclareAsync(
                exchange: "payment_exchange",
                type: "direct",
                durable: true,
                autoDelete: false
            );

            return new PaymentPublisher(connection, channel);
        }

        public async Task PublishPaymentSucceededAsync(Contracts.Events.PaymentSucceeded evt)
        {
            await PublishAsync(evt, "payment.succeeded");
        }

        public async Task PublishPaymentFailedAsync(Contracts.Events.PaymentFailed evt)
        {
            await PublishAsync(evt, "payment.failed");
        }

        private async Task PublishAsync<T>(T evt, string routingKey)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));

            var messageBody = JsonSerializer.Serialize(evt);
            var body = Encoding.UTF8.GetBytes(messageBody);

            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body
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
                Console.WriteLine($"[PaymentPublisher] Dispose error: {ex.Message}");
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
