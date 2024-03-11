using API.Data;
using API.Helpers;
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

            // Adding Cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // Adding Photo Service
            services.AddScoped<IPhotoService, PhotoService>();


            return services;
        }
    }
}
