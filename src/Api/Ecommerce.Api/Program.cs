using Ecommerce.Api.Middleware;
using Ecommerce.Catalog.Application.DependencyInjection;
using Ecommerce.Catalog.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(
    builder.Configuration.GetConnectionString("Catalog")
        ?? "Server=(localdb)\\mssqllocaldb;Database=EcommerceCatalog;Trusted_Connection=True;TrustServerCertificate=True");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
