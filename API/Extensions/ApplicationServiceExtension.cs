using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Conection with database
            services.AddDbContext<DataContext>(opt => {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // Adding cors
            services.AddCors();

            // Adding the token
            services.AddScoped<ITokenService, TokenService>();
            
            // Adding Repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Adding AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 


            return services;
        }
    }
}
