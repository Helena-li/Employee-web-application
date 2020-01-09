using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using EmployeeWebApplication.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeWebApplication
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                            options.SignIn.RequireConfirmedEmail = true)
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();
            services.AddAuthentication().AddGoogle(option =>
            {
                option.ClientId = "926758641945-kps0kt3vt33i72tga4ntte6bu975gdfe.apps.googleusercontent.com";
                option.ClientSecret = "1oD57l05FDRL1vfwCE4TI17S";
                option.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                option.ClaimActions.Clear();
                option.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                option.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                option.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                option.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                option.ClaimActions.MapJsonKey("urn:google:profile", "link");
                option.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            })
            .AddFacebook(option =>
            {
                option.ClientId = "2453599471557470";
                option.ClientSecret = "1a357d1a6d9a0687f16ebda2aaaaaae5";
            });

            services.AddAuthorization(option =>
            {
                option.AddPolicy("EditRepository",
                    policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOhterAdminRolesAndClaimsHandler>();
            services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = new PathString("/admin/AccessDenied");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
