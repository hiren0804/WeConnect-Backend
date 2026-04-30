using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;
using WeConnect.Infrastructure.Repositories;
using WeConnect.Infrastructure.Services;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Application.Services;

namespace WeConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Master DB (tenant registry + shared tables) ─────────────
        services.AddDbContext<MasterDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString("Master"),
                npgsql =>
                {
                    npgsql.MigrationsAssembly(
                        typeof(MasterDbContext).Assembly.FullName);
                    npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
                    npgsql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
            .UseSnakeCaseNamingConvention()
        );

        // ── Tenant DB factory (creates context per request, per slug) ──
        services.AddScoped<TenantDbContextFactory>();

        // ── Repositories ────────────────────────────────────────────
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantConnectionRepository, TenantConnectionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IWidgetRepository, WidgetRepository>();

        // IContentRepository and IListingRepository are created per-request
        // by controllers using the per-request TenantDbContext from TenantMiddleware.

        // ── Application Services ─────────────────────────────────────

        services.AddMemoryCache();


        services.AddScoped<TemplateService>();
        services.AddScoped<WidgetService>();
        services.AddScoped<ListingService>();
        // ── Provisioning ────────────────────────────────────────────
        services.AddScoped<ProvisioningService>();

        return services;
    }
}
