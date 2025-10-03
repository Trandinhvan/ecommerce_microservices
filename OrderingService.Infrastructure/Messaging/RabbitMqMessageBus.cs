using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OrderingService.Application.Interfaces;

public class RabbitMqMessageBus : IAsyncDisposable, IMessageBus
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchangeName = "ordering_exchange";
    private readonly string _queueName = "ordering_queue";
    private bool _disposed;

    private RabbitMqMessageBus(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    // Factory async để tạo instance
    public static async Task<RabbitMqMessageBus> CreateAsync()
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

        // Khai báo exchange và queue (async API)
        await channel.ExchangeDeclareAsync(
            exchange: "ordering_exchange",
            type: "direct",
            durable: true,
            autoDelete: false
        );

        await channel.QueueDeclareAsync(
            queue: "ordering_queue",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await channel.QueueBindAsync(
            queue: "ordering_queue",
            exchange: "ordering_exchange",
            routingKey: "order.created"
        );

        return new RabbitMqMessageBus(connection, channel);
    }

    public async Task PublishAsync<T>(T message, string routingKey)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var messageBody = JsonSerializer.Serialize(message);
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

    // Subscribe async
    public async Task SubscribeAsync<T>(string routingKey, Action<T> handler)
    {
        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: routingKey
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<T>(message);

                handler(deserializedMessage);

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xử lý message: {ex.Message}");
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
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
            Console.WriteLine($"Lỗi khi giải phóng tài nguyên RabbitMQ: {ex.Message}");
        }
        finally
        {
            if (_channel != null) await _channel.DisposeAsync();
            if (_connection != null) await _connection.DisposeAsync();
            _disposed = true;
        }
    }
}
