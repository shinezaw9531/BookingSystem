using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Hangfire;
using Infrastructure.DatabaseContexts;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastruture(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. MSSQL & EF Core
            services.AddDbContext<BookingSystemContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            // 2. Redis
            //var redisConnection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection"));
            //services.AddSingleton<IConnectionMultiplexer>(redisConnection);

            // 3. Hangfire
            services.AddHangfire(config => config
                // ... Hangfire configuration options
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddHangfireServer();

            // 4. Register Repositories and External Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IWaitlistRepository, WaitlistRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();

            services.AddScoped<IExternalEmailService, MockEmailService>();
            services.AddScoped<IPaymentService, MockPaymentService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IExternalEmailService, MockEmailService>();

            return services;
        }
    }
}
