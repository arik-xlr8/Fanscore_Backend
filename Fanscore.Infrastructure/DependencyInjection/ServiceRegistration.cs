using FanScore.Application.Interfaces.Repositories;
using FanScore.Application.Interfaces.Services;
using FanScore.Infrastructure.Data;
using FanScore.Infrastructure.Repositories;
using FanScore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FanScore.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bulunamadı.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 36))
                ));

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}