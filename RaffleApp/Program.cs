using Azure.Identity;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights;
using RaffleApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load configuration and feature flags from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    var endpoint = builder.Configuration["AZURE_APPCONFIGURATION_ENDPOINT"];
    if (!string.IsNullOrEmpty(endpoint))
    {
        options.Connect(new Uri(endpoint), new DefaultAzureCredential())
               .Select("RaffleApp:*")
               .UseFeatureFlags(featureFlagOptions =>
               {
                   featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(5));
               });
    }
});

// Integrate Azure App Configuration middleware
builder.Services.AddAzureAppConfiguration();

// Add Application Insights telemetry with adaptive sampling disabled.
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions() { EnableAdaptiveSampling = false })
                .AddSingleton<ITelemetryInitializer, TargetingTelemetryInitializer>();

// Add feature management, targeting, and telemetry services
builder.Services.AddFeatureManagement()
                .WithTargeting()
                .AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add anonymous user tracking middleware before authorization
app.UseMiddleware<AnonymousUserTrackingMiddleware>();

// Use Azure App Configuration middleware
app.UseAzureAppConfiguration();

// Add targeting context middleware before feature evaluation
app.UseMiddleware<TargetingHttpContextMiddleware>();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
