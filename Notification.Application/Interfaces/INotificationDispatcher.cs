using Notification.Domain.Entities;
using Notification.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(UserNotification notification);
        Task HandleOrderCreatedAsync(OrderCreatedEvent e);
        Task HandleOrderPaidAsync(OrderPaidEvent e);

    }
}
