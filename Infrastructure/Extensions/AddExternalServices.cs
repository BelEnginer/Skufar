using Infrastructure.Database;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;


namespace Infrastructure.Extensions;

public static class ExternalServices
{
    public static void AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetRequiredConnectionString("Database");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbConnectionString));
        var cacheConnectionString = configuration.GetRequiredConnectionString("Redis"); 
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheConnectionString;
            options.InstanceName = "AppCache:";
        });
        services.Configure<MinioSetting>(configuration.GetSection("ConnectionStrings:Minio"));
        services.AddSingleton<IMinioClient>(options =>
        {
            var minioSettings = options.GetRequiredService<IOptions<MinioSetting>>().Value;
            if (string.IsNullOrWhiteSpace(minioSettings.Endpoint) ||
                string.IsNullOrWhiteSpace(minioSettings.AccessKey) ||
                string.IsNullOrWhiteSpace(minioSettings.SecretKey))
            {
                throw new InvalidOperationException("MinIO configuration is missing required parameters.");
            }
            return new MinioClient()
                .WithEndpoint(minioSettings.Endpoint)
                .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
                .WithSSL(minioSettings.UseSSL)
                .Build();
        });
    }

    private static string GetRequiredConnectionString(this IConfiguration configuration, string key)
    {
        var value = configuration.GetConnectionString(key);
        if (value == null)
        {
            throw new InvalidOperationException($"Connection string '{key}' is missing in configuration.");
        }
        return value;
    }
}