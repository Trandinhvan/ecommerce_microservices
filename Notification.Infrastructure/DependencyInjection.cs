using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Interfaces;
using Notification.Application.Services;
using Notification.Infrastructure.Email;
using Notification.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IEmailSender, MailKitEmailSender>();
            //services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();
            // services.AddScoped<ISmsSender, TwilioSmsSender>(); // nếu cần

            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            services.AddHostedService<RabbitMqListener>();
            return services;
        }
    }
}
