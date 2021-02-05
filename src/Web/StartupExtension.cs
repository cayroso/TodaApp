using App.CQRS;
using App.Services;
using Data.App.Models.Trips;
using Data.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.BackgroundServices;

namespace Web
{
    public static class StartupExtension
    {
        public static void RegisterCQRS(IServiceCollection services)
        {
            services.AddSingleton<JobQueue<Trip>>();
            services.AddHostedService<MyJobBackgroundService>();

            services.AddTransient<IContainer, DotNetCoreContainer>();
            services.AddScoped<ITenantProvider, DefaultTenantProvider>();
            services.AddScoped<IAppDbContextFactory, DefaultAppDbContextFactory>();
            services.AddHttpContextAccessor();

            RegisterCore(services);

            //RegisterCommonCQRS(services);

            services.AddCommandQueryHandlers(typeof(ICommandHandler<>));
            services.AddCommandQueryHandlers(typeof(IQueryHandler<,>));
        }

        static void RegisterCore(IServiceCollection services)
        {
            services.AddScoped<ISequentialGuidGenerator, DefaultSequentialGuidGenerator>();

            services.AddTransient<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddTransient<ICommandHandlerDispatcher, DefaultCommandHandlerDispatcher>();

            services.AddTransient<IQueryHandlerFactory, DefaultQueryHandlerFactory>();
            services.AddTransient<IQueryHandlerDispatcher, QueryHandlerDispatcher>();

        }


        static void AddCommandQueryHandlers(this IServiceCollection services, Type handlerInterface)
        {
            var handlers = typeof(App.CQRS.IContainer).Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
            );

            foreach (var handler in handlers)
            {
                foreach (var impl in handler.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == handlerInterface))
                {
                    services.AddScoped(impl, handler);
                }


            }
        }

    }

    public class DotNetCoreContainer : IContainer
    {
        private readonly IServiceProvider _serviceProvider;

        public DotNetCoreContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        T IContainer.Resolve<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
