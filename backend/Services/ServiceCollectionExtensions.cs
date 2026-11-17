using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KeyManagement.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var serviceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Name.EndsWith("Service"));

        foreach (var serviceType in serviceTypes)
        {
            services.AddScoped(serviceType);
        }

        services.AddScoped<IAuditLogService>(provider =>
            provider.GetRequiredService<AuditLogService>());

        return services;
    }
}