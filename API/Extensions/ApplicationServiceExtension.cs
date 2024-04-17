using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Adding cors
            services.AddCors();

            // Adding the token
            services.AddScoped<ITokenService, TokenService>();
            
            // Adding AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

            // Adding Cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // Adding Photo Service
            services.AddScoped<IPhotoService, PhotoService>();

            //Adding LastActivity Action Filters
            services.AddScoped<LogUserActivity>();

            // Adding SignalR
            services.AddSignalR();

            // Adding Presence Tracker
            services.AddSingleton<PresenceTracker>();
            
            // Unit to Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
