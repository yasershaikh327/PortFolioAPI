using DataAccess.AppSettings;
using DataAccess.Helper;
using DataAccess.Mappers;
using DataAccess.Repositories;
using DataAccess.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortFolioAPI.DataAccess;
using PortFolioAPI.GlobalExceptionMiddleware;
using System.Text.Json;
using System.Threading.RateLimiting;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on the Vercel port
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = Environment.GetEnvironmentVariable("DEFAULTCONNECTION");
string ISPROD = Environment.GetEnvironmentVariable("ISPROD");

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<INotificationService,NotificationService>();
builder.Services.AddTransient<IHelper, Helper>();
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddSlidingWindowLimiter("public-api", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { Status = 429, Error = "Too many requests" });
        await context.HttpContext.Response.WriteAsync(payload, token);
    };

});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Error/Error", "?statusCode={0}");

app.UseRateLimiter();

//app.MapControllers().RequireRateLimiting("public-api");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
