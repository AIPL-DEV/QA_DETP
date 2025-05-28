using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DETP.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.UseRowNumberForPaging;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;
using DETP.requests.editObservation;
using DETP.services;
using System.Configuration;

namespace DETP
{
    public class Startup
    {

        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment Env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddSession();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "dd-MM-yyyy";

            });

            services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = 1000_0000_0000);
            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer(
                      Configuration.GetConnectionString("DefaultConnection"), opt => opt.UseRowNumberForPaging())
                );


            var builder = services.AddRazorPages();

            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    option =>
                    {
                        option.LoginPath = new PathString("/Account/Login");
                        option.AccessDeniedPath = new PathString("/denied");
                    });

            AddServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory factory)
        {
            factory.AddFile(Path.Combine("c:", "logs", "DETP", "{Date}.txt"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    "...",                                              // Route name
                    "Account/Validate/{id}",                           // URL with parameters
                    new { controller = "Account", action = "Validate", id = "" }  // Parameter defaults
                );
                routes.MapRoute(
                    name: "Account",
                    template: "{controller=Account}");

                routes.MapRoute(
                    name: "Request",
                    template: "{controller=Request}");

                routes.MapRoute(
                    name: "Job",
                    template: "{controller=Job}");

                routes.MapRoute(
                    name: "User",
                    template: "{controller=User}");

                routes.MapRoute(
                    name: "AddUser",
                    template: "{controller=AddUser}");

                routes.MapRoute(
                    name: "AddDepartment",
                    template: "{controller=AddDepartment}");

                routes.MapRoute(
                    name: "QAObservation",
                    template: "{controller=QAObservation}");

                routes.MapRoute(
                    name: "ViewObservation",
                    template: "{controller=ViewObservation}");

                routes.MapRoute(
                    name: "EditObservation",
                    template: "{controller=EditObservation}");

                routes.MapRoute(
                     "....",                                              // Route name
                     "EditObservation/Submit/{id}",                           // URL with parameters
                     new { controller = "EditObservation", action = "Submit", id = "" }  // Parameter defaults
                 );

                routes.MapRoute(
                "del",                                              // Route name
                "User/Delete/{id}",                           // URL with parameters
                new { controller = "User", action = "Delete", id = "" }  // Parameter defaults
                );



            });

            app.Use((context, next) =>
            {
                context.Response.Headers.Add("The X-Frame-Options", "SAMEORIGIN");
                return next.Invoke();
            });
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddScoped<ObservationFlowService>();
            BaseService.Configure(Configuration);
        }
    }
}
