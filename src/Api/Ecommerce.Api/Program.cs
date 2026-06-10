using System.Text;
using Ecommerce.Api.Middleware;
using Ecommerce.Auth.Application.DependencyInjection;
using Ecommerce.Auth.Infrastructure.DependencyInjection;
using Ecommerce.Auth.Infrastructure.Security;
using Ecommerce.Catalog.Application.DependencyInjection;
using Ecommerce.Catalog.Infrastructure.DependencyInjection;
using Ecommerce.Api.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var jwtOptions = builder.Configuration.GetSection("Auth:Jwt").Get<JwtOptions>()
    ?? new JwtOptions
    {
        Issuer = "Ecommerce.Api",
        Audience = "Ecommerce.Api",
        SigningKey = "development-only-change-me-minimum-32-characters",
        AccessTokenLifetimeMinutes = 15
    };

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the JWT access token only. Swagger UI sends: Authorization: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(
    builder.Configuration.GetConnectionString("Auth")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceAuth;Trusted_Connection=True;TrustServerCertificate=True",
    jwtOptions);
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(
    builder.Configuration.GetConnectionString("Catalog")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceCatalog;Trusted_Connection=True;TrustServerCertificate=True");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
