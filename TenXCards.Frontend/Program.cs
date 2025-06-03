using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using TenXCards.Frontend.Components;
using TenXCards.Frontend.Configuration;
using TenXCards.Frontend.Services;
using TenXCards.Frontend.Services.Handlers;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<JwtConfiguration>(
//    builder.Configuration.GetSection(JwtConfiguration.SectionName));
builder.Services.Configure<APIConfiguration>(
    builder.Configuration.GetSection(APIConfiguration.SectionName));

builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddHttpContextAccessor();

//builder.Services.AddBlazoredSessionStorage();
//builder.Services.AddBlazoredLocalStorage();
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//    options.IdleTimeout = TimeSpan.FromHours(1);
//    options.Cookie.SameSite = SameSiteMode.Strict;
//});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
/*builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();*/

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
//app.UseSession();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
