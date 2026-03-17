using Auth.Core.Extensions;
using Auth.Keycloak.Extensions;
using Crm.Api.Middleware;
using Crm.Api.Swagger;
using Crm.Application.Auth;
using Crm.Application.Users;
using Crm.Infrastructure.Database;
using Crm.Infrastructure.Database.Extensions;
using Crm.Infrastructure.Import;
using Crm.Infrastructure.Keycloak.Extensions;
using Crm.Infrastructure.Logging;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

SwaggerSettings.AddLocker(builder);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();
builder.AddSerilogLogging();


builder.Services.AddCrmAuth(builder.Configuration);
builder.Services.AddKeycloakJwtAuthentication(builder.Configuration);
builder.Services.AddCrmKeycloakAdmin(builder.Configuration);

builder.Services.AddCrmPersistence(builder.Configuration);
builder.Services.AddScoped<ShoppingTrendsImportService>();
builder.Services.AddScoped<RoleSeeder>();

builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseSerilogRequestLoggingMiddleware();
app.UseGlobalExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.SeedRolesAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
