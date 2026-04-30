// WeConnect.Application/DependencyInjection.cs
namespace WeConnect.Application;
using Microsoft.Extensions.DependencyInjection;
using WeConnect.Application.Services;
using WeConnect.Application.Common.Interfaces;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
       services.AddMemoryCache();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ContentService>();
        return services;
    }
}