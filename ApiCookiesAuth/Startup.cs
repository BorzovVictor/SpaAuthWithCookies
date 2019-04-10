using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCookiesAuth.Data;
using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiCookiesAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<IdentityUser>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = PwSettings.PasswordRequiredLength;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = false;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.User.AllowedUserNameCharacters = null;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<SimpleContext>();
            builder.AddRoleValidator<RoleValidator<IdentityRole>>();
            builder.AddRoleManager<RoleManager<IdentityRole>>();
            builder.AddSignInManager<SignInManager<IdentityUser>>();

            services.AddMvc();
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("fully permissive",
                    configurePolicy => configurePolicy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
                        .AllowCredentials());
            });

            services.AddDbContext<PwContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("mssql")));
            
            services.AddDbContext<SimpleContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("mssql")));


            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                })
                .AddGoogle("Google", options =>
                {
                    options.CallbackPath = new PathString("/google-callback");
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    options.Events = new OAuthEvents
                    {
                        OnRemoteFailure = (RemoteFailureContext context) =>
                        {
                            context.Response.Redirect("/home/denied");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddFacebook("Facebook", options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    options.Events = new OAuthEvents
                    {
                        OnRemoteFailure = (RemoteFailureContext context) =>
                        {
                            context.Response.Redirect("/home/denied");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddTwitter("Twitter", options =>
                {
                    options.CallbackPath = new PathString("/twitter_callback");
                    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                    options.Events = new TwitterEvents
                    {
                        OnRemoteFailure = (RemoteFailureContext context) =>
                        {
                            context.Response.Redirect("/home/denied");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });
            ;

            services.AddTransient<PwSeeder>();

            Mapper.Reset();
            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Seed the database
                //using (var scope = app.ApplicationServices.CreateScope())
                //{
                //    var seeder = scope.ServiceProvider.GetService<PwSeeder>();
                //    seeder.Seed().Wait();
                //}
            }

            app.UseAuthentication();

            app.UseCors("fully permissive");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // app.UseMvcWithDefaultRoute();

            app.UseMvc(cfg =>
            {
                cfg.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Fallback", action = "Index" }
                );
            });
        }
    }
}