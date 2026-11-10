using Microsoft.EntityFrameworkCore;
using SupportDashboard.Api.Data;
using SupportDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Render (and most PaaS hosts) inject PORT; fall back to 8080 for local Docker.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=support.db"));

builder.Services.AddScoped<ITicketService, TicketService>();

var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

var defaultOrigins = new[]
{
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "https://magentrix-desk.onrender.com"
};

var allowedOrigins = configuredOrigins
    .Concat(defaultOrigins)
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

// Swagger stays available in production so the live API is easy to demo.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.MapControllers();

app.Run();
