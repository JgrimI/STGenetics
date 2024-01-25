using STGenetics.Server;
using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("MyClient", c =>
{
    c.BaseAddress = new Uri("https://localhost:7023");
});



var app = builder.Build();
app.Lifetime.ApplicationStarted.Register(() =>
{
    var environment = app.Environment;
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Hosting environment: {environment}", environment.EnvironmentName);
    logger.LogInformation("Now listening on: {address}", string.Join(", ", app.Urls));
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.Run();
