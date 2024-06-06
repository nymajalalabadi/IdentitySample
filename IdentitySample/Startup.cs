using IdentitySample.Authorization.ClaimBasedAuthorization;
using IdentitySample.Models;
using IdentitySample.Models.Context;
using IdentitySample.Repositories;
using IdentitySample.Security.Default;
using IdentitySample.Security.DynamicRole;
using IdentitySample.Security.PhoneTotp;
using IdentitySample.Security.PhoneTotp.Providers;
using Kaktos.UserImmediateActions.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersianTranslation.Identity;
using System;
using IdentitySample.Services;
using IdentitySample.Quartz;
using IdentitySample.Quartz.JobFactories;
using IdentitySample.Quartz.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;


namespace IdentitySample
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            #region Db Context

            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            });

            #endregion

            #region google

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "";
                    options.ClientSecret = "";
                });


            #endregion

            #region Identity

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 0;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>()
                .AddUserImmediateActions() //sing out
                .AddPermanentImmediateActionsStore<ApplicationPermanentImmediateActionsStore>(); //zekhirie sazi daemi

            #endregion

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/AccessDenied";
                options.Cookie.Name = "IdentityProj";
                options.LoginPath = "/Login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            services.Configure<SecurityStampValidatorOptions>(option =>
            {
                option.ValidationInterval = TimeSpan.FromMinutes(30);
            });

            #region Claim

            services.AddAuthorization(option =>
            {
                option.AddPolicy("EmployeeListPolicy",
                    policy => policy.RequireClaim(ClaimTypesStore.EmployeeList, false.ToString(), true.ToString()));

                option.AddPolicy("ClaimOrRole", policy =>
                     policy.RequireAssertion(ClaimOrRole));

                option.AddPolicy("ClaimRequirement", policy =>
                    policy.Requirements.Add(new ClaimRequirement(ClaimTypesStore.EmployeeList, true.ToString())));

                option.AddPolicy("DynamicRole", policy =>
                    policy.Requirements.Add(new DynamicRoleRequirement()));
            });

            #endregion

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddTransient<IUtilities, Utilities>();
            services.AddScoped<IMessageSender, MessageSender>();
            services.AddSingleton<IAuthorizationHandler, ClaimHandler>();
            services.AddSingleton<IAuthorizationHandler, DynamicRoleHandler>();

            services.AddTransient<IPhoneTotpProvider, PhoneTotpProvider>();
            services.Configure<PhoneTotpOptions>(options =>
            {
                options.StepInSeconds = 30;
            });

            services.AddClaimBasedAuthorization();

            // Add quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<Quartz.QuartzHostedService>();
            services.AddSingleton<ImmediateActionsJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ImmediateActionsJob),
                cronExpression: "0 0 0/6 1/1 * ? *"));

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseUserImmediateActions();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private bool ClaimOrRole(AuthorizationHandlerContext context)
        {
           return context.User.HasClaim(ClaimTypesStore.EmployeeList, true.ToString()) ||
              context.User.IsInRole("Admin");
        }
    }
}
