using Blazor.Auth;
using Blazor.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using System.Net.Http;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpClient<AuthService>("api");
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvide>();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredLocalStorage();


// Register HttpClient for server-side Blazor
builder.Services.AddHttpClient();  // Registers HttpClient to be available for dependency injection
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Place it after Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add UseAntiforgery middleware back in the correct position
app.UseAntiforgery();

// Ensure it is before UseEndpoints or MapControllers
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();