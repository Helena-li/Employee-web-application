using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using EmployeeWebApplication.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebAppAccess", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build();
                });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver
                    = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                            options.SignIn.RequireConfirmedEmail = true)
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();
            services.AddAuthentication().AddGoogle(option =>
            {
                option.ClientId = "***";
                option.ClientSecret = "***";
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
                option.ClientId = "***";
                option.ClientSecret = "***";
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

            services.AddSingleton<DataProtectionPurposeString>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddJwt(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseCors("AllowWebAppAccess");
            
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
