using LexiFlow.Domain.Interfaces;
using LexiFlow.Infrastructure.ExternalServices;
using LexiFlow.Infrastructure.Persistence;
using LexiFlow.Infrastructure.Repositories;
using LexiFlow.Infrastructure.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexiFlow.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWordRepository, WordRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IReviewHistoryRepository, ReviewHistoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IFirebaseAuthService, FirebaseService>();

        var geminiBaseUrl = configuration["GeminiApi:BaseUrl"] ?? "https://generativelanguage.googleapis.com";
        
        services.AddHttpClient<IGeminiService, GeminiService>(client =>
        {
            client.BaseAddress = new Uri(geminiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddPolicyHandler(ResiliencePolicies.GetRetryPolicy())
        .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

        return services;
    }
}
