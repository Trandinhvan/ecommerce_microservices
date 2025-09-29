using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Enum;
using Notification.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Services
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEmailSender _email;
        private readonly IRealtimeNotifier _realtime;
        private readonly ISmsSender? _sms;

        public NotificationDispatcher(IEmailSender email, IRealtimeNotifier realtime, ISmsSender? sms = null)
        {
            _email = email;
            _realtime = realtime;
            _sms = sms;
        }

        public async Task DispatchAsync(UserNotification n)
        {
            // Email
            await _email.SendAsync(
                to: n.UserId, // giả định UserId là email; production: map từ UserId -> email
                subject: $"[Ecommerce] {n.Title}",
                htmlBody: $"<h3>{n.Title}</h3><p>{n.Message}</p><small>{n.CreatedAt:u}</small>");

            // Realtime
            await _realtime.NotifyUserAsync(n.UserId, $"{n.Title}: {n.Message}");
            // SMS (tùy)
            // if (_sms != null) await _sms.SendAsync(userPhone, $"{n.Title}: {n.Message}");
        }

        public async Task HandleOrderCreatedAsync(OrderCreatedEvent e)
        {
            var n = new UserNotification
            {
                UserId = e.UserId,
                Type = NotificationType.OrderCreated,
                Title = "Đơn hàng đã tạo",
                Message = $"Đơn #{e.OrderId} tổng {e.TotalPrice:C} đã được tạo thành công."
            };
            await DispatchAsync(n);
        }

        public async Task HandleOrderPaidAsync(OrderPaidEvent e)
        {
            var n = new UserNotification
            {
                UserId = e.UserId,
                Type = NotificationType.OrderPaid,
                Title = "Thanh toán thành công",
                Message = $"Đơn #{e.OrderId} đã được thanh toán {e.Amount:C}."
            };
            await DispatchAsync(n);
        }
    }
}
