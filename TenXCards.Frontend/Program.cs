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
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorization();//.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddAuthentication();

//tymczasowo off
//builder.Services.AddIdentityCore<ApplicationUser>()
//    .AddUserStore<ApplicationUserStore>()
//.AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
//.AddSignInManager();
//.AddDefaultTokenProviders();
//;
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    // Cookie settings
//    options.Cookie.HttpOnly = true;
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

//    options.LoginPath = "/Identity/Account/Login";
//    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//    options.SlidingExpiration = true;
//});

builder.Services.AddBlazoredSessionStorage();

builder.Services.AddTransient<ErrorHandlingHttpMessageHandler>();
builder.Services.AddHttpClient("API", client =>
                    {    
                        var apiUrl = builder.Configuration.GetValue<string>("API:Url");
                        client.BaseAddress = new Uri(apiUrl!);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJyb2JlcnRAY3VycmVuZGEucGwiLCJmaXJzdE5hbWUiOiJSb2JlcnQiLCJsYXN0TmFtZSI6Ikt1a2xhIiwianRpIjoiYTJlMmRjZTktYjc5Mi00NGU0LWI0NmQtNTBiYzVjMWIzZjAxIiwiZXhwIjoxNzQ4MzI5MDAxLCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.XGZAZlNLurQO7ts2_P7YU83yFO8XjDn_JLPA1yKFLmw");
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
