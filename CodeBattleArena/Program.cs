using CodeBattleArena.Controllers;
using CodeBattleArena.Data;
using CodeBattleArena.Service;
using CodeBattleArena.Service.DBServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddSingleton<ConnectionMapping>();
builder.Services.AddTransient<Notification>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();

//
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время неактивности до истечения срока действия сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<DBPlayerService>();
builder.Services.AddScoped<DBSessionService>();
builder.Services.AddScoped<DBChatService>();
builder.Services.AddScoped<DBTaskService>();
builder.Services.AddScoped<DBService>();

builder.Services.AddHostedService<DatabaseCheckService>();

var connectionStringBD = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => {
    options.LogTo(Console.WriteLine);
    options.UseSqlServer(connectionStringBD);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // Размер файла в байтах
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomePage}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
