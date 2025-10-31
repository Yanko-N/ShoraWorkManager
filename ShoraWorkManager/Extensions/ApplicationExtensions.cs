using Application.Core;
using Application.Data.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Models;

namespace ShoraWorkManager.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            var retryIntervalSeconds = config.GetValue<int>("Configurations:DbRetryOnFailureTimeSpanFromSeconds");
            var retryCount = config.GetValue<int>("Configurations:DbRetryOnFailureNumber");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBContext' not found."),
                        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(retryCount, TimeSpan.FromSeconds(retryIntervalSeconds), null))
                );

            services.AddIdentity<User,IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Registar MediaR através da assembly
            var applicationAssembly = typeof(AppSettings).Assembly;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

            services.AddMemoryCache();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Account/Logout";
            });

            return services;
        }

        

        public static WebApplication AddWebApplicationExtras(this WebApplication application)
        {
            SeedRolesExtension(application);

            return application;
        }

        private static WebApplication SeedRolesExtension(this WebApplication application)
        {
            using (var scope = application.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                SeedRoles.Seed(roleManager);
                return application;
            }
        }
    }
}

