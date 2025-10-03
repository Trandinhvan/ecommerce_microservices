using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Application.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, string routingKey);
        Task SubscribeAsync<T>(string routingKey, Action<T> handler);
    }
}
