using Microsoft.EntityFrameworkCore;
using OvenLog.Infrastructure.Data;
using OvenLog.Infrastructure.Services;
using OvenLog.Web.Components;
using OvenLog.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure SQLite database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=ovenlog.db";
builder.Services.AddDbContext<OvenLogDbContext>(options =>
    options.UseSqlite(connectionString));

// Register application services
builder.Services.AddScoped<OvenLogService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<BarcodeInputService>();
builder.Services.AddScoped<AppStateService>();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OvenLogDbContext>();
    await SeedData.InitializeAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
