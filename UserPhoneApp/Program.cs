using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Data;
using UserPhoneApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

/*
 * Database on SQLite
 */
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

/*
 * UserService - for Users
 * Phones Service - for Phones
 */
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPhoneService, PhoneService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();

