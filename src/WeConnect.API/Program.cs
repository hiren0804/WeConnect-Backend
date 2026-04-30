using Asp.Versioning;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using WeConnect.API.Middleware;
using WeConnect.Application;
using WeConnect.Infrastructure;
using WeConnect.Infrastructure.Services;
using WeConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "WeConnect-Backend")
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ── Core Services ────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── API Controllers (Backend Only) ─────────────────────────────────────
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── MediatR ──────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// ── Authentication (custom token for now) ─────────────────────────
builder.Services.AddAuthentication("HardcodedToken")
    .AddScheme<AuthenticationSchemeOptions, HardcodedTokenAuthHandler>(
        "HardcodedToken", null);

// ── Rate Limiting ────────────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddInMemoryRateLimiting();

// ── API Versioning ───────────────────────────────────────────────
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

// ── CORS (keep your original logic) ──────────────────────────────
builder.Services.AddCors(o => o.AddPolicy("Dev", p => p
    .SetIsOriginAllowed(origin =>
    {
        var host = new Uri(origin).Host;
        return host.EndsWith(".localhost") || host == "localhost";
    })
    .AllowAnyMethod()
    .AllowAnyHeader()));

// ── OpenAPI (.NET 10) ────────────────────────────────────────────
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Dev Only: DB Migration + Seeder + Swagger ────────────────────

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var master       = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
    var tenantFactory = scope.ServiceProvider.GetRequiredService<TenantDbContextFactory>();
    var provisioning  = scope.ServiceProvider.GetRequiredService<ProvisioningService>(); // ← add

    await master.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(master, tenantFactory, provisioning); // ← pass it

    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// ── Middleware Pipeline (VERY IMPORTANT ORDER) ───────────────────
app.UseMiddleware<ExceptionMiddleware>();     // 1. Global error handling
app.UseSerilogRequestLogging();               // 2. Logging
app.UseIpRateLimiting();                     // 3. Rate limiting
app.UseCors("Dev");                          // 4. CORS
app.UseMiddleware<TenantMiddleware>();       // 5. Tenant resolution

app.UseHttpsRedirection();

app.UseAuthentication();                     // 6. Auth
app.UseAuthorization();                      // 7. Authorization

// ── Routes ──────────────────────────────────────────────────────
app.MapGet("/", () => Results.Ok("WeConnect backend running 🚀"));
app.MapControllers();

app.Run();
