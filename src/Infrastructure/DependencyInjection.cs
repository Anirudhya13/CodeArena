using Microsoft.SemanticKernel;
using CodeArena.Application.Common.Interfaces;
using CodeArena.Application.Common.Interfaces;
using CodeArena.Infrastructure.Data;
using CodeArena.Infrastructure.Data.Interceptors;
using CodeArena.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlite(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        var groqApiKey = builder.Configuration["Groq:ApiKey"];
        groqApiKey = string.IsNullOrWhiteSpace(groqApiKey) ? "dummy-key" : groqApiKey;

        var httpClient = new HttpClient(new CodeArena.Infrastructure.AI.GroqEndpointHandler(new HttpClientHandler()));

        builder.Services.AddKernel()
            .AddOpenAIChatCompletion(
                modelId: "openai/gpt-oss-120b",
                apiKey: groqApiKey,
                httpClient: httpClient);

        builder.Services.AddScoped<IAiJudgeService, CodeArena.Infrastructure.AI.SemanticKernelJudgeService>();
    }
}








