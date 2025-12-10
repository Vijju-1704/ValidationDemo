using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/account/login";
                    options.LogoutPath = "/account/logout";
                    options.AccessDeniedPath = "/account/accessdenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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

            app.UseAuthorization();
            //app.MapGet("/", () => Results.Redirect("/user/register"));
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Register}/{id?}");

            app.Run();
        }
    }
}
