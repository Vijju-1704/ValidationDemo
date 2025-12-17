using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ValidationDemo.Authorization;
using ValidationDemo.Constants;
using ValidationDemo.Data;
using ValidationDemo.Filters;
using ValidationDemo.Repositories;
using ValidationDemo.Services;

namespace ValidationDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                options.Filters.Add<GlobalTimeFilter>();
                options.Filters.Add<LogActionFilter>();
            });
            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigureHttpClient(client =>
            {
               client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register Service
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSession();
            // Filters
            builder.Services.AddScoped<GlobalExceptionFilter>();
            builder.Services.AddScoped<PageVisitTimeFilter>();
            builder.Services.AddScoped<GlobalTimeFilter>();
            builder.Services.AddScoped<LogActionFilter>();
            // Memory Cache for the PageVisitTimeFilter
            builder.Services.AddMemoryCache();

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Authentication with Cookies
            _ = builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
           .AddCookie(options =>
           {
               options.LoginPath = "/account/login";
               options.LogoutPath = "/account/logout";
               options.AccessDeniedPath = "/account/accessdenied";
               options.ExpireTimeSpan = TimeSpan.FromHours(1);
               options.SlidingExpiration = true;
               options.Cookie.HttpOnly = true;
               options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
           })
           .AddGoogle(googleOptions =>
           {
               googleOptions.ClientId += builder.Configuration["Authentication:Google:ClientId"];
               googleOptions.ClientSecret += builder.Configuration["Authentication:Google:ClientSecret"];
               googleOptions.CallbackPath = "/signin-google";
               googleOptions.SaveTokens = true;
           });

            builder.Services.AddAuthorization(options =>
            {
                // Policy 1: Admin Only
                options.AddPolicy(AppPolicies.AdminOnly, policy =>
                    policy.RequireRole(AppRoles.Admin));

                // Policy 2: Can View Deleted Users (Admin only)
                options.AddPolicy(AppPolicies.CanViewDeletedUsers, policy =>
                    policy.RequireRole(AppRoles.Admin)
                          .RequireClaim(AppClaims.CanViewDeletedUsers, "true"));

                // Policy 3: Can Manage Users (Admin only)
                options.AddPolicy(AppPolicies.CanManageUsers, policy =>
                    policy.RequireRole(AppRoles.Admin)
                          .RequireClaim(AppClaims.CanDeleteUsers, "true")
                          .RequireClaim(AppClaims.CanEditUsers, "true"));
            });

            // Register custom authorization handler
            builder.Services.AddSingleton<IAuthorizationHandler, CanEditOwnProfileHandler>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            builder.Services.AddSession();
            app.UseSession();
            //app.MapGet("/", () => Results.Redirect("/user/register"));
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Register}/{id?}");
            app.Run();
        }
    }
}
