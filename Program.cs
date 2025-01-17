using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmApp;
using Jinaga;
using ViewModels.Todo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddSingleton(CreateJinagaClient)
    .AddScoped<TodoViewModel>();

await builder.Build().RunAsync();

JinagaClient CreateJinagaClient(IServiceProvider provider)
{
    return JinagaClient.Create(opt =>
    {
    });
}
