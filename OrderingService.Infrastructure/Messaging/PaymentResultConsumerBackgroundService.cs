//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrderingService.Infrastructure.Messaging
//{
//    public class PaymentResultConsumerBackgroundService : BackgroundService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly IConnection _connection;
//        private readonly IChannel _channel;

//        public PaymentResultConsumerBackgroundService(IServiceScopeFactory scopeFactory)
//        {
//            _scopeFactory = scopeFactory;

//            // Khởi tạo kết nối RabbitMQ
//            var factory = new ConnectionFactory()
//            {
//                HostName = "localhost",
//                DispatchConsumersAsync = true
//            };
//            _connection = factory.CreateConnection();
//            _channel = _connection.CreateChannel();

//            // Đảm bảo queue tồn tại
//            _channel.QueueDeclare(
//                queue: "payment-result-queue",
//                durable: true,
//                exclusive: false,
//                autoDelete: false,
//                arguments: null
//            );
//        }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var consumer = new AsyncEventingBasicConsumer(_channel);

//            consumer.Received += async (model, ea) =>
//            {
//                try
//                {
//                    var body = ea.Body.ToArray();
//                    var json = Encoding.UTF8.GetString(body);

//                    var evt = JsonSerializer.Deserialize<PaymentResultMessage>(json);

//                    if (evt != null)
//                    {
//                        // Tạo scope mới để resolve scoped service (IOrderUseCase)
//                        using var scope = _scopeFactory.CreateScope();
//                        var orderUseCase = scope.ServiceProvider.GetRequiredService<IOrderUseCase>();

//                        var status = evt.IsSuccess ? OrderStatus.Paid : OrderStatus.Cancelled;

//                        await orderUseCase.UpdateOrderStatusAsync(evt.OrderId, status);
//                    }

//                    // Xác nhận đã xử lý
//                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"[PaymentResultConsumer] Error: {ex.Message}");
//                    // Không ack để có thể retry
//                }
//            };

//            _channel.BasicConsume(
//                queue: "payment-result-queue",
//                autoAck: false,
//                consumer: consumer
//            );

//            return Task.CompletedTask;
//        }

//        public override void Dispose()
//        {
//            _channel.Close();
//            _connection.Close();
//            base.Dispose();
//        }
//}
