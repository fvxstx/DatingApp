using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task ClearConnections(DataContext context){
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager){
            if(await  userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
            
            var roles = new List<AppRole>{
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach(var role in roles){
                await roleManager.CreateAsync(role);
            };

            if(users == null) return;

            foreach (var user in users){
                user.UserName = user.UserName!.ToLower();
                user.Photos![0].IsApproved = true;
                
                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);

                await userManager.CreateAsync(user, "Pa$$w0rddd");
                await userManager.AddToRoleAsync(user, "Member");
            };

            var admin = new AppUser {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rddd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }
    }
}