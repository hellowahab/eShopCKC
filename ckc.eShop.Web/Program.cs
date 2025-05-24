using Ckc.EShop.Infrastructure.Identity;
using eShopCKC.Infrastructure;
using eShopCKC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShopCKC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<ICatalogService, CatalogService>();

            builder.Services.AddDbContext
                <CatalogDbContext>(
                    c =>
                    {
                        try
                        {
                            c.UseSqlServer("Server=YouServerName;Integrated Security=true;Initial Catalog=CKC.eShopOnWeb.Catalog;Encrypt=False;");
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                );

            builder.Services.AddDbContext
                <AppIdentityDbContext>(
                    c =>
                    {
                        try
                        {
                            c.UseSqlServer("Server=YouServerName;Integrated Security=true;Initial Catalog=CKC.eShopOnWeb.Identity;Encrypt=False;");
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Catalog}/{action=Index}/{id?}")
                .WithStaticAssets();

            CatalogContextSeed.SeedAsync(app,
                app.Services.GetRequiredService<ILoggerFactory>())
                .Wait();

            AppIdentityDbContextSeed.SeedAsync(app.Services).GetAwaiter().GetResult();

            app.Run();
        }
    }
}
