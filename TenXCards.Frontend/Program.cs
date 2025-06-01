using TenXCards.Frontend.Components;
using MudBlazor;
using MudBlazor.Services;
using TenXCards.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using TenXCards.API.Configuration;
using TenXCards.Frontend.Configuration;
using System;
using Blazored.SessionStorage;
using TenXCards.Frontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<APIConfiguration>(
//    builder.Configuration.GetSection(APIConfiguration.SectionName));

// Add services to the container.
//builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();      //wróciæ do tej koncepcji
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

//???
//builder.Services.AddBlazoredSessionStorage();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromSeconds(10); // Set session timeout
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

//////////tymczasowo off
//////////builder.Services.AddIdentityCore<ApplicationUser>()
//////////    .AddUserStore<ApplicationUserStore>()
//////////.AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
//////////.AddSignInManager();
//////////.AddDefaultTokenProviders();
//////////;
//////////builder.Services.ConfigureApplicationCookie(options =>
//////////{
//////////    // Cookie settings
//////////    options.Cookie.HttpOnly = true;
//////////    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
////////
//////////    options.LoginPath = "/Identity/Account/Login";
//////////    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//////////    options.SlidingExpiration = true;
//////////});

builder.Services.AddTransient<ErrorHandlingHttpMessageHandler>();
builder.Services.AddHttpClient("API", client =>
{
    var apiUrl = builder.Configuration.GetValue<string>("API:Url");
    client.BaseAddress = new Uri(apiUrl!);
})
                .AddHttpMessageHandler<ErrorHandlingHttpMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
