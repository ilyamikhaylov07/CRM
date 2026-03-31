using Auth.Core.Extensions;
using Auth.Keycloak.Extensions;
using Crm.Api.Middleware;
using Crm.Api.Swagger;
using Crm.Application.Activities;
using Crm.Application.Auth;
using Crm.Application.Clients;
using Crm.Application.DealItems;
using Crm.Application.Deals;
using Crm.Application.Forecasts;
using Crm.Application.Helpers.RecommendationDataSeed;
using Crm.Application.ML;
using Crm.Application.ML.Forecasting;
using Crm.Application.ML.Recommendations;
using Crm.Application.Products;
using Crm.Application.Recommendations;
using Crm.Application.Tasks;
using Crm.Application.Users;
using Crm.Infrastructure.Database;
using Crm.Infrastructure.Database.Extensions;
using Crm.Infrastructure.HealthChecks;
using Crm.Infrastructure.Import;
using Crm.Infrastructure.Keycloak.Extensions;
using Crm.Infrastructure.Logging;
using Crm.Infrastructure.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
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
builder.Services.AddCrmObservability(
    builder.Configuration,
    "Crm.Api",
    Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0");

builder.Services.AddCrmAuth(builder.Configuration);
builder.Services.AddKeycloakJwtAuthentication(builder.Configuration);
builder.Services.AddCrmKeycloakAdmin(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<KeycloakHealthCheck>("keycloak", tags: new[] { "ready" })
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

builder.Services.AddCrmPersistence(builder.Configuration);
builder.Services.AddScoped<ShoppingTrendsImportService>();
builder.Services.AddScoped<RoleSeeder>();

builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDealAmountCalculator, DealAmountCalculator>();
builder.Services.AddScoped<IDealItemService, DealItemService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IForecastService, ForecastService>();
builder.Services.AddScoped<IRecommendationDataSeedHelper, RecommendationDataSeedHelper>();
builder.Services.AddScoped<IForecastTrainingDataService, ForecastTrainingDataService>();
builder.Services.AddScoped<IForecastModelService, ForecastModelService>();
builder.Services.AddScoped<IRecommendationTrainingDataService, RecommendationTrainingDataService>();
builder.Services.AddScoped<IRecommendationModelService, RecommendationModelService>();
builder.Services.AddScoped<IHeuristicRecommendationScorer, HeuristicRecommendationScorer>();
builder.Services.AddScoped<IRecommendationEngineService, RecommendationEngineService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IMachineLearningOrchestrator, MachineLearningOrchestrator>();

var app = builder.Build();

app.UseSerilogRequestLoggingMiddleware();
app.UseGlobalExceptionHandling();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.UseCrmObservability();
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
