using App.Hubs;
using Data.App.DbContext;
using Data.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Middlewares;

namespace Web
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
            services.AddDbContext<AppDbContext>(opt => { });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RouteOptions>(opt =>
            {
                opt.LowercaseUrls = true;
                opt.AppendTrailingSlash = true;
            });

            services.AddLogging();

            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                //options.PayloadSerializerSettings.ContractResolver = new CamelCaseContractResolver();
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //options.PayloadSerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mmZ";
                //options.PayloadSerializerSettings.Culture = cultureInfo;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationRoles.AdministratorRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.AdministratorRoleName
                           )));

                options.AddPolicy(ApplicationRoles.DriverRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.DriverRoleName
                           )));

                options.AddPolicy(ApplicationRoles.RiderRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.RiderRoleName
                           )));


                //options.AddPolicy(ApplicationPermissions.ManageUserTasks, policy =>
                //   policy.RequireAssertion(context =>
                //       context.User.HasClaim(c =>
                //       c.Type == System.Security.Claims.ClaimTypes.Role &&
                //        (c.Value == ApplicationRoles.ManagerRoleName || c.Value == ApplicationRoles.AssistantRoleName)
                //           )));
            });


            var mvcBuilder = services.AddRazorPages(opt =>
            {

            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddRazorPagesOptions(opt =>
            {
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.AdministratorRoleName, "/", ApplicationRoles.AdministratorRoleName);
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.DriverRoleName, "/", ApplicationRoles.DriverRoleName);
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.RiderRoleName, "/", ApplicationRoles.RiderRoleName);
            });



            //services.AddTransient<Hub<IUserTaskHubClient>, UserTaskHub>();
            //services.AddTransient<UserTaskHubClient>();

            //services.AddTransient<Hub<IOrderHubClient>, OrderHub>();
            //services.AddTransient<OrderHubClient>();

            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
            });

#if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

#if !DEBUG
            services.AddProgressiveWebApp();
#endif
            services.AddScoped<App.Services.ChatService>();

            services.AddTransient<ChatHub>();

            StartupExtension.RegisterCQRS(services);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/administrator/"))
                {
                    context.Request.Path = "/administrator/";
                }
                else if (context.Request.Path.Value.StartsWith("/driver/"))
                {
                    context.Request.Path = "/driver/";
                }
                else if (context.Request.Path.Value.StartsWith("/rider/"))
                {
                    context.Request.Path = "/rider/";
                }


                //context.Response.ContentType = "text/html";
                //await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));

                await next();
            });

            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
