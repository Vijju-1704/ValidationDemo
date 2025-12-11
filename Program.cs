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
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
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
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });
            builder.Services.AddAuthorization(options =>
            {
                // ===== ROLE-BASED POLICIES =====

                // Policy 1: Require Admin Role
                options.AddPolicy("RequireAdmin", policy =>
                    policy.RequireRole("Admin"));

                // Policy 2: Require Admin OR Manager Role
                options.AddPolicy("RequireAdminOrManager", policy =>
                    policy.RequireRole("Admin", "Manager"));

                // Policy 3: Require Manager Role
                options.AddPolicy("RequireManager", policy =>
                    policy.RequireRole("Manager"));

                // ===== PERMISSION-BASED POLICIES =====

                // Policy 4: Can Delete Users
                options.AddPolicy("CanDeleteUsers", policy =>
                    policy.RequireClaim("Permission", "DeleteUsers"));

                // Policy 5: Can Edit Users
                options.AddPolicy("CanEditUsers", policy =>
                    policy.RequireClaim("Permission", "EditUsers"));

                // Policy 6: Can View Reports
                options.AddPolicy("CanViewReports", policy =>
                    policy.RequireClaim("Permission", "ViewReports"));

                // ===== AGE-BASED POLICIES =====

                // Policy 7: Must be 18 or older
                options.AddPolicy("MustBe18", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var ageClaim = context.User.FindFirst("Age");
                        if (ageClaim == null)
                            return false;

                        return int.Parse(ageClaim.Value) >= 18;
                    }));

                // Policy 8: Must be 21 or older
                options.AddPolicy("MustBe21", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var ageClaim = context.User.FindFirst("Age");
                        if (ageClaim == null)
                            return false;

                        return int.Parse(ageClaim.Value) >= 21;
                    }));

                // ===== DEPARTMENT-BASED POLICIES =====

                // Policy 9: Must be in IT Department
                options.AddPolicy("MustBeIT", policy =>
                    policy.RequireClaim("Department", "IT"));

                // Policy 10: Must be in HR Department
                options.AddPolicy("MustBeHR", policy =>
                    policy.RequireClaim("Department", "HR"));

                // ===== COMBINED POLICIES =====

                // Policy 11: Admin in IT Department
                options.AddPolicy("AdminInIT", policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("Department", "IT");
                });

                // Policy 12: Manager with Edit Permission
                options.AddPolicy("ManagerWithEditPermission", policy =>
                {
                    policy.RequireRole("Manager");
                    policy.RequireClaim("Permission", "EditUsers");
                });
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
            //app.MapGet("/", () => Results.Redirect("/user/register"));
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Register}/{id?}");

            app.Run();
        }
    }
}
