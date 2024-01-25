using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.LoadingIndicator; 
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STGenetics.Client;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
builder.Services.AddBlazoredToast();
builder.Services.AddAuthorizationCore();
builder.Services
    .AddBlazorise(options =>
    {
        options.Debounce = true;
    })
    .AddBootstrap5Providers().AddLoadingIndicator()
    .AddFontAwesomeIcons();
builder.Services.AddScoped<ILoadingService, LoadingService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress), Timeout = TimeSpan.FromMinutes(20) });

await builder.Build().RunAsync();
