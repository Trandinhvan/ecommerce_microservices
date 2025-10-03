using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;
using Notification.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Notification.Infrastructure.Messaging
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IConfiguration _config;
        private readonly INotificationDispatcher _dispatcher;
        private IConnection? _conn;
        private IModel? _channel;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:UserName"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();

            _channel.QueueDeclare("order_events", durable: true, exclusive: false, autoDelete: false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var type = ea.BasicProperties.Type; // "OrderCreated" | "OrderPaid"

                    if (type == "OrderCreated")
                    {
                        var msg = JsonSerializer.Deserialize<OrderCreatedEvent>(json)!;
                        await _dispatcher.HandleOrderCreatedAsync(msg);
                    }
                    else if (type == "OrderPaid")
                    {
                        var msg = JsonSerializer.Deserialize<OrderPaidEvent>(json)!;
                        await _dispatcher.HandleOrderPaidAsync(msg);
                    }

                    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message");
                    // Nack + requeue
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel!.BasicConsume("order_events", autoAck: false, consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _conn?.Close();
            base.Dispose();
        }
    }
}
