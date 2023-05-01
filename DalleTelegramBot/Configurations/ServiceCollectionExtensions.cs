using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Data.Repositories;
using DalleTelegramBot.Services.OpenAI;
using DalleTelegramBot.Services.Telegram;
using DalleTelegramBot.Services.Telegram.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Configurations
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>(provider =>
            {
                string cs = configuration.GetConnectionString("CS")
                ?? throw new NullReferenceException("An error occurred while retrieving the connection string from the application configuration file. Please check that the 'CS' key has been defined with a valid connection string and try again");
                return new(cs);
            });
        }

        public static void AddTelegram(this IServiceCollection services, TelegramSettings settings)
        {
            services.Configure<ReceiverOptions>(options =>
            {
                options.AllowedUpdates = new[] { UpdateType.Message, UpdateType.InlineQuery, UpdateType.CallbackQuery };
                options.ThrowPendingUpdates = true;
            });

            services.AddHttpClient("telegram_bot")
                .AddTypedClient<ITelegramBotClient>((client) =>
                {
                    var telegramOptions = new TelegramBotClientOptions(settings.Token);
                    return new TelegramBotClient(telegramOptions, client);
                });


            services.AddScoped<TelegramUpdateHandler>();
         
            services.AddScoped<ReceiverService>();

            services.AddScoped<ITelegramService, TelegramService>();
        }

        public static void AddOpenAIClient(this IServiceCollection services, OpenAISettings settings)
        {
            services.AddHttpClient("openai_client")
                .AddTypedClient<IOpenAIClient>(c =>
            {
                return new OpenAIClient(settings, c);
            });
        }

        public static void AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<RateLimitingMemoryCache>();
            services.AddSingleton<StateManagementMemoryCache>();
        }

        public static void InjectAutomatically(this IServiceCollection services)
        {
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType);
            Type scopedType = typeof(IScopedDependency), transientType = typeof(ITransientDependency), singletonType = typeof(ISingletonDependency);
            foreach (var serviceType in serviceTypes)
            {
                var implementedInterfaces = serviceType.GetInterfaces();
                if (implementedInterfaces.Any(scopedType.IsAssignableFrom))
                {
                    services.AddScoped(serviceType);
                }
                else if (implementedInterfaces.Any(transientType.IsAssignableFrom))
                {
                    services.AddTransient(serviceType);
                }
                else if (implementedInterfaces.Any(singletonType.IsAssignableFrom))
                {
                    services.AddSingleton(serviceType);
                }
            }
        }
    }
}
