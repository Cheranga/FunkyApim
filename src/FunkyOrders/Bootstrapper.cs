using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FunkyOrders.Core.Http;
using FunkyOrders.Features.CreateOrder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FunkyOrders;

public static class Bootstrapper
{
    public static IHost GetHost(
        Action<HostBuilderContext, IServiceCollection>? customRegistrations = null,
        Action<HostBuilderContext, IConfigurationBuilder>? customConfigurations = null
    )
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(builder =>
            {
                var services = builder.Services;
                services.Configure<JsonSerializerOptions>(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.PropertyNameCaseInsensitive = true;
                    options.WriteIndented = false;
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            })
            .ConfigureAppConfiguration(
                (context, builder) =>
                {
                    builder.AddUserSecrets<Program>();

                    customConfigurations?.Invoke(context, builder);
                }
            )
            .ConfigureServices(
                (context, services) =>
                {
                    services.AddSingleton(
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = false,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        }
                    );
                    services.AddSingleton(typeof(IApiRequestReader<,>), typeof(ApiRequestReader<,>));
                    services.AddSingleton<IOrderApiResponseGenerator, OrderApiResponseGenerator>();
                    services.AddSingleton<IOrderProcessor, OrderProcessor>();
                    services.AddValidatorsFromAssembly(typeof(Program).Assembly);

                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();

                    customRegistrations?.Invoke(context, services);
                }
            )
            .ConfigureLogging(
                (context, logging) =>
                {
                    logging.Services.Configure<LoggerFilterOptions>(options =>
                    {
                        var defaultRule = options.Rules.FirstOrDefault(rule =>
                            string.Equals(
                                rule.ProviderName,
                                "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider",
                                StringComparison.OrdinalIgnoreCase
                            )
                        );
                        if (defaultRule is not null)
                        {
                            options.Rules.Remove(defaultRule);
                        }
                    });
                }
            )
            .Build();

        return host;
    }
}
