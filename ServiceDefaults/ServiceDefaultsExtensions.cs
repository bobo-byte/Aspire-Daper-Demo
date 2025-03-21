namespace ServiceDefaults;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public static class ServiceDefaultsExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder)
    {
        // Add service discovery
        builder.Services.AddServiceDiscovery();

        // Add resilience for HTTP clients
        builder.Services.AddHttpClient().AddResilienceEnricher();

        // Add logging
        builder.Services.AddLogging();

        // Add OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        // Add health checks
        builder.Services.AddHealthChecks();

        return builder;
    }
}