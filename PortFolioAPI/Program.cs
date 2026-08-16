using DataAccess.Mappers;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortFolioAPI.DataAccess;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on the Vercel port
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));  
builder.Services.AddScoped<IMapper, Mapper>();  
builder.Services.AddScoped<IRepository, Repository>();  
builder.Services.AddScoped<INotificationService,NotificationService>();
// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("public-api", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;   // 10-second segments
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();

app.UseStatusCodePages();

app.MapControllers().RequireRateLimiting("public-api");

app.MapGet("/", () => Results.Ok(new
{
    status = "OK",
    message = "PortFolioAPI is running"
}));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
