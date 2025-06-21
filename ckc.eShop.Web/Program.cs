using Ckc.EShop.ApplicationCore;
using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.ApplicationCore.Services;
using Ckc.EShop.Infrastructure.Data;
using Ckc.EShop.Infrastructure.Identity;
using Ckc.EShop.Web.Interfaces;
using Ckc.EShop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Ckc.EShop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddTransient<ICatalogService, CatalogService>();

            builder.Services.AddScoped<IBasketService, BasketService>();

            builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("Catalog"));

            builder.Services.AddSingleton<IUriComposer>(sp =>
            {
                var catalogSettings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                return new UriComposer(catalogSettings);
            });


            builder.Services.AddDbContext
                <CatalogDbContext>(
                    c =>
                    {
                        try
                        {
                            c.UseInMemoryDatabase("CKC.eShopOnWeb.Catalog");
                            //c.UseSqlServer("Server=DESKTOP-D1CJQPN;Integrated Security=true;Initial Catalog=CKC.eShopOnWeb.Catalog;Encrypt=False;");
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
                            c.UseInMemoryDatabase("CKC.eShopOnWeb.Identity");
                            //c.UseSqlServer("Server=DESKTOP-D1CJQPN;Integrated Security=true;Initial Catalog=CKC.eShopOnWeb.Identity;Encrypt=False;");
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

            builder.Services.AddScoped(typeof(IRepository<>),typeof(EFRepository<>));

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

            app.UseSession();

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
