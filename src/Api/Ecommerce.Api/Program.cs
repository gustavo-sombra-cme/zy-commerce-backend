using System.Text;
using Ecommerce.Api.Assistant;
using Ecommerce.Api.Assistant.TextToSql;
using Ecommerce.Api.HealthChecks;
using Ecommerce.Api.Middleware;
using Ecommerce.Api.Mcp;
using Ecommerce.Api.OpenApi;
using Ecommerce.Api.Security;
using Ecommerce.Auth.Domain.Users;
using Ecommerce.Auth.Application.DependencyInjection;
using Ecommerce.Auth.Infrastructure.DependencyInjection;
using Ecommerce.Auth.Infrastructure.Persistence;
using Ecommerce.Auth.Infrastructure.Security;
using Ecommerce.Catalog.Application.DependencyInjection;
using Ecommerce.Catalog.Infrastructure.DependencyInjection;
using Ecommerce.Catalog.Infrastructure.Persistence;
using Ecommerce.Orders.Application.DependencyInjection;
using Ecommerce.Orders.Infrastructure.DependencyInjection;
using Ecommerce.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

const string LiveHealthCheckTag = "live";
const string ReadyHealthCheckTag = "ready";
const string AllowAllCorsPolicyName = "AllowAll";

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("Auth:Jwt").Get<JwtOptions>()
    ?? new JwtOptions
    {
        Issuer = "Ecommerce.Api",
        Audience = "Ecommerce.Api",
        SigningKey = "development-only-change-me-minimum-32-characters",
        AccessTokenLifetimeMinutes = 15
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAllCorsPolicyName, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<AssistantLlmOptions>(
    builder.Configuration.GetSection(AssistantLlmOptions.SectionName));
builder.Services.Configure<AssistantTextToSqlOptions>(
    builder.Configuration.GetSection(AssistantTextToSqlOptions.SectionName));
builder.Services.AddScoped<AssistantOrchestrator>();
builder.Services.AddSingleton<AssistantSafetyPolicy>();
builder.Services.AddSingleton<AssistantIntentRouter>();
builder.Services.AddSingleton<AssistantIntentPlanValidator>();
builder.Services.AddSingleton<AssistantIntentPlanJsonParser>();
builder.Services.AddSingleton<DeterministicAssistantIntentInterpreter>();
builder.Services.AddHttpClient<HttpAssistantLlmClient>();
builder.Services.AddHttpClient<GeminiAssistantLlmClient>();
builder.Services.AddScoped<IAssistantLlmClient>(services =>
{
    var options = services.GetRequiredService<IOptions<AssistantLlmOptions>>().Value;
    return options.IsGeminiProvider
        ? services.GetRequiredService<GeminiAssistantLlmClient>()
        : services.GetRequiredService<HttpAssistantLlmClient>();
});
builder.Services.AddScoped<LlmAssistantIntentInterpreter>();
builder.Services.AddScoped<IAssistantIntentInterpreter>(services =>
{
    var options = services.GetRequiredService<IOptions<AssistantLlmOptions>>().Value;
    return options.Enabled
        ? services.GetRequiredService<LlmAssistantIntentInterpreter>()
        : services.GetRequiredService<DeterministicAssistantIntentInterpreter>();
});
builder.Services.AddSingleton<AssistantToolRegistry>();
builder.Services.AddSingleton<AssistantSqlValidator>();
builder.Services.AddSingleton<IAssistantSqlConnectionFactory, AssistantSqlConnectionFactory>();
builder.Services.AddScoped<IAssistantReadOnlySqlExecutor, AssistantReadOnlySqlExecutor>();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AuthorizationHeaderOperationFilter>();
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
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Ecommerce.Api.Authentication");

                logger.LogWarning(
                    "JWT bearer authentication failed with {ExceptionType} for {RequestMethod} {RequestPath}.",
                    context.Exception.GetType().Name,
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path.Value);

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Ecommerce.Api.Authentication");

                logger.LogInformation(
                    "JWT bearer challenge issued for {RequestMethod} {RequestPath} with error {AuthenticationError}.",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path.Value,
                    context.Error);

                return Task.CompletedTask;
            }
        };

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
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(UserRole.Admin.ToString());
    });
});

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .AddAuthorizationFilters()
    .WithTools<EcommerceMcpTools>();

builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { LiveHealthCheckTag })
    .AddCheck<DbContextHealthCheck<AuthDbContext>>("auth-database", tags: new[] { ReadyHealthCheckTag })
    .AddCheck<DbContextHealthCheck<CatalogDbContext>>("catalog-database", tags: new[] { ReadyHealthCheckTag })
    .AddCheck<DbContextHealthCheck<OrdersDbContext>>("orders-database", tags: new[] { ReadyHealthCheckTag });

builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(
    builder.Configuration.GetConnectionString("Auth")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceAuth;Trusted_Connection=True;TrustServerCertificate=True",
    jwtOptions);

builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(
    builder.Configuration.GetConnectionString("Catalog")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceCatalog;Trusted_Connection=True;TrustServerCertificate=True");

builder.Services.AddOrdersApplication();
builder.Services.AddOrdersInfrastructure(
    builder.Configuration.GetConnectionString("Orders")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceOrders;Trusted_Connection=True;TrustServerCertificate=True");

var app = builder.Build();

app.UseRouting();

app.UseCors(AllowAllCorsPolicyName);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMcp("/mcp").RequireAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains(LiveHealthCheckTag)
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains(ReadyHealthCheckTag)
});

app.Run();
