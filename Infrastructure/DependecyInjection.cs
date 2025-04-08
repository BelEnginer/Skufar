using System.Text;
using Application.Abstractions.IServices;
using Infrastructure.Database;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddSingleton<ITokenCacheService, TokenCacheService>();
            services.AddSingleton<IChatCacheService, ChatCacheService>();
            AddPersistence(services, configuration);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.PostgreSQL(connectionString: configuration.GetConnectionString("Database"),
                    tableName: "logs",
                    needAutoCreateTable: true,
                    columnOptions: new Dictionary<string, ColumnWriterBase>
                    {
                        { "timestamp", new TimestampColumnWriter() },
                        { "level", new LevelColumnWriter() },
                        { "message", new RenderedMessageColumnWriter() },
                        { "exception", new ExceptionColumnWriter() },
                        { "properties", new LogEventSerializedColumnWriter() }
                    })
                .CreateLogger();
            var key = configuration["JWT:Key"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT:Key is not set in the configuration.");
            }
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(key))
                        };
                });
            return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
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