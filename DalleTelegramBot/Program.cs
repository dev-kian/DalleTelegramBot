using DalleTelegramBot;
using DalleTelegramBot.Common;
using DalleTelegramBot.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

try
{
    ConfigureLogging();
    Log.Information("Application starting");
    await CreateHostBuilderAsync();
}
catch (Exception ex)
{
    Log.Error(ex, "An unhandled exception occurred.");
}
finally
{
    await Log.CloseAndFlushAsync();
}


async Task CreateHostBuilderAsync()
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(config =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog();
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<AppSettings>(context.Configuration.GetSection(nameof(AppSettings)));
            var settings = context.Configuration.GetRequiredSection(nameof(AppSettings)).Get<AppSettings>()
                ?? throw new NullReferenceException("Application settings could not be loaded.");

            services.AddDbContext(context.Configuration);

            services.AddOpenAIClient(settings.OpenAISettings);

            services.AddCaching();

            services.AddTelegram(settings.TelegramSettings);

            services.InjectAutomatically();

            services.AddHostedService<HostedService>();
        })
        .RunConsoleAsync();
}

void ConfigureLogging()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Filter.ByExcluding(Matching.FromSource("System.Net.Http.HttpClient"))
        .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "log-.log"),
            LogEventLevel.Information, fileSizeLimitBytes: (1024 * 1024 * 1), rollingInterval: RollingInterval.Day)
        .WriteTo.Console()
        .CreateLogger();
}