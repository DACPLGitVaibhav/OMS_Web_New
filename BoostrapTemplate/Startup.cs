using DATA;
using DATA.Interaces;
using DATA.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OMS_Template.ViewModels.OPCUA;
using Opc.Ua.Client;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate
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
            services.AddRazorPages();
            services.AddControllers().AddRazorRuntimeCompilation();
            //For DB Connection
            services.AddDbContext<ContextClass>(x => x.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            //For Coookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.ExpireTimeSpan = TimeSpan.FromHours(24);
                x.LoginPath = "/Account/LoginPage";
                x.AccessDeniedPath = "/Account/AccessDenied";
            });

            //For Sessions
            services.AddSession(x =>
            {
                x.IdleTimeout = TimeSpan.FromMinutes(20);
                x.Cookie.HttpOnly = true;
                x.Cookie.IsEssential = true;
            });


            //Dependancy Injection
            services.AddScoped<IUser, UserSerivces>();
            services.AddScoped<ILightBill, LightBillServices>();
            services.AddScoped<ICustDetails, CustDetailsServices>();
            services.AddScoped<ILineMaster, LineServices>();
            services.AddScoped<IOrders, OrdersServices>();
            services.AddTransient<ILOTDetials, LOTDetailsServices>();
            services.AddSingleton<OpcUaClientService>();
           services.AddScoped<IVarriantCode,VarriantCodeService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/ErrorPage");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Defualt", pattern: "{controller=Account}/{action=loginpage}/{id?}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
