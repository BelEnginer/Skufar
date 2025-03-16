using System.Text;
using Application.Abstractions.IServices;
using Application.IServices;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog.Sinks.PostgreSQL;

namespace Infrastructure.Extensions;
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddSingleton<ITokenCacheService, TokenCacheService>();
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
        }
    }