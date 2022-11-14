using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IFileService, FileService>();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x=>x.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(3);
})
     .AddEntityFrameworkStores<AppDbContext>();
var app = builder.Build();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=account}/{action=login}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}"
    );

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedAsync(userManager, roleManager);
} 
app.Run();
