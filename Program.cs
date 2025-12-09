using Microsoft.EntityFrameworkCore;
using ValidationDemo.Data;
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
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register Service
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSession();

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
            app.MapGet("/", () => Results.Redirect("/user/register"));
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Register}/{id?}");

            app.Run();
        }
    }
}
