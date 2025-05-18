using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TenXCards.API.Configuration;
using TenXCards.API.Data;
using TenXCards.API.Jwt;
using TenXCards.API.Options;
using TenXCards.API.Services;
using TenXCards.API.Services.OpenRouter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
//builder.Services.AddHttpClient<OpenRouterService>((serviceProvider, client) =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
//    var baseUrl = configuration["OpenRouter:ApiEndpoint"]
//        ?? "https://openrouter.ai/api/v1/";

//    client.BaseAddress = new Uri(baseUrl);
//    client.DefaultRequestHeaders.Add("User-Agent", "FlashCards/1.0");
//});
// .AddPolicyHandler((serviceProvider, _) => 
// {
//     var logger = serviceProvider.GetRequiredService<ILogger<OpenRouterService>>();
//     return RetryPolicies.GetHttpRetryPolicy(logger);
// });


builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection(RateLimitOptions.SectionName));
builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.SectionName));
builder.Services.Configure<AIServiceOptions>(    
    builder.Configuration.GetSection(AIServiceOptions.SectionName));

// Register necessary services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MainDatabase")));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
builder.Services.AddScoped<IErrorLoggingService, ErrorLoggingService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration[$"{JwtOptions.SectionName}:Issuer"],
            ValidAudience = builder.Configuration[$"{JwtOptions.SectionName}:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration[$"{JwtOptions.SectionName}:SecretKey"]))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();
