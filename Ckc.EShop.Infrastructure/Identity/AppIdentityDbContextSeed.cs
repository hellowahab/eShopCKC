using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ckc.EShop.Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            context.Database.Migrate();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string username = "demouser@microsoft.com";
            string password = "Pass@word1";

            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null) return;

            var defaultUser = new ApplicationUser
            {
                UserName = username
            };

            var result = await userManager.CreateAsync(defaultUser, password);
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine($"Code: {error.Code} Description: {error.Description}");
                }
                throw new Exception($" Failed to create default user: \n {sb}");
            
            }





        
        }
    }
}
