using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace JoyfulReaperLib.JRAspNet.Identity
{
    public static class IdentitySeeder
    {
        public static async void EnsurePopulated<T>(WebApplication app) where T : IdentityDbContext
        {
            IdentityDbContext context = app.Services
                .CreateScope().ServiceProvider
                .GetRequiredService<T>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            UserManager<IdentityUser> userManager = app.Services
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            IdentityUser user = await userManager.FindByNameAsync(app.Configuration.GetSection("IdentitySettings")["AdminUser"]);
            if (user == null)
            {
                user = new IdentityUser(app.Configuration.GetSection("IdentitySettings")["AdminUser"]);
                user.Email = app.Configuration.GetSection("IdentitySettings")["AdminUser"];

                await userManager.CreateAsync(user, app.Configuration.GetSection("IdentitySettings")["AdminPassword"]);
            }
        }
    }
}
