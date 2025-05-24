using TenXCards.Frontend.Components;
using MudBlazor;
using MudBlazor.Services;
using TenXCards.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using TenXCards.API.Configuration;
using TenXCards.Frontend.Configuration;
using System;
using Blazored.SessionStorage;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<APIConfiguration>(
//    builder.Configuration.GetSection(APIConfiguration.SectionName));

// Add services to the container.
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
//builder.Services.AddAuthorizationCore();


builder.Services.AddBlazoredSessionStorage();

builder.Services.AddTransient<ErrorHandlingHttpMessageHandler>();
builder.Services.AddHttpClient("API", client =>
{    
    var apiUrl = builder.Configuration.GetValue<string>("API:Url");
    client.BaseAddress = new Uri(apiUrl!);
    client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJyb2JlcnRAY3VycmVuZGEucGwiLCJmaXJzdE5hbWUiOiJSb2JlcnQiLCJsYXN0TmFtZSI6Ikt1a2xhIiwianRpIjoiMTQwYjI2YzYtYTVjYi00NDVjLWFhZTEtNzA0OWNmN2M3ODRlIiwiZXhwIjoxNzQ4MDg3MDc3LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.buoA5j8xiKIVAZ6CVhAnW4amW2n2pI2hGWx4l1AE6Jw");
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
