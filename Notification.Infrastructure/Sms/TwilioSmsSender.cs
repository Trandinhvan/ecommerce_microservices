using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Sms
{
    public class TwilioSmsSender
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            // TODO: tích hợp Twilio (AccountSid/AuthToken)
            return Task.CompletedTask;
        }
    }
}
