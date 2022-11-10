using Fiorello.DAL;
using Fiorello.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IFileService, FileService>();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x=>x.UseSqlServer(connectionString));

var app = builder.Build();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}"
    );
app.UseStaticFiles();   
app.Run();
