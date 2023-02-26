using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if(users==null) return;
            var roles = new List<AppRole>
            {
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
            var admin = new AppUser
            {
                UserName = "admin"
            };
            admin.Created = DateTime.UtcNow;
            admin.Created = admin.Created.ToUniversalTime();
            admin.LastActive = DateTime.UtcNow;
            admin.LastActive = admin.LastActive.ToUniversalTime();
            var result = await userManager.CreateAsync(admin, "Pa$$w0rd");
            if (!result.Succeeded) {
                throw new ArgumentException("Parameter cannot be null");
            }
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            foreach (var user in users)
            {
                user.Created = DateTime.UtcNow;
                user.Created = user.Created.ToUniversalTime();
                user.LastActive = DateTime.UtcNow;
                user.LastActive = user.LastActive.ToUniversalTime();
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }
        }
    }
}