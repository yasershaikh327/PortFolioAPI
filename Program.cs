var builder = WebApplication.CreateSlimBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();
app.MapGet("/", () => Results.Json(new { message = "Hello from .NET on Vercel" }));
app.MapGet("/health", () => Results.Json(new { status = "ok" }));
app.Run();