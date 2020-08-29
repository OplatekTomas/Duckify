using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Duckify.Areas.Identity;
using Duckify.Data;
using Duckify.Models;
using Duckify.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Duckify {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            //services.AddDbContext<SongContext>(options =>
            //s    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(options => {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages(options => {
                
            });
            services.AddSingleton<SpotifyService>();
            services.AddSingleton<SongQueue>();
            services.AddServerSideBlazor();
            //services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services
                .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>
                >();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider srv) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            CreateAdminAccount(srv).Wait();
        }
        
        
        private async Task CreateAdminAccount(IServiceProvider serviceProvider) {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count > 0) {
                return;
            }
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames) {
                if (!await roleManager.RoleExistsAsync(roleName)) {
                    _ = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var email = Configuration["AdminUsername"];
            if (email == null) {
                do {
                    Console.WriteLine("Please enter admin email: ");
                    email = Console.ReadLine();
                } while (!Helper.IsValidEmail(email));
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) {
                user = new User() {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var pwd = Configuration["AdminPassword"];
                if (pwd == null) {
                    Console.WriteLine("Please enter admin password: ");
                    pwd = Console.ReadLine();
                }
                await userManager.CreateAsync(user, pwd);
                
            }
            await userManager.AddToRoleAsync(user, "Admin");

        }
        
    }
}