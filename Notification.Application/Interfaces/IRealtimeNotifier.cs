using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface IRealtimeNotifier
    {
        Task NotifyUserAsync(string userId, string message);
    }
}
