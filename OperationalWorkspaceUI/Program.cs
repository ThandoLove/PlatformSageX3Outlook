using FluentValidation;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspaceApplication.Orchestration;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.Interfaces.BackgroundJobsApp;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceApplication.Validators;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
using OperationalWorkspaceInfrastructure.servicesInfra;
using OperationalWorkspaceInfrastructure.servicesInfra.BackgroundWorkers;
using OperationalWorkspaceUI.Components;
using OperationalWorkspaceUI.Security;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Actions;
using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceUI.UIServices.EmailService;
using OperationalWorkspaceUI.UIServices.System;
using OperationalWorkspaceUI.UIServices.ToastUIService;
using OperationalWorkspaceUI.UIServices.Workspace;
using Radzen;
using System;
using System.Linq;
using System.Net.Http;
using ToastService = OperationalWorkspaceUI.UIServices.ToastUIService.ToastService;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1. SYSTEM
// ======================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

// ======================================================
// 2. AUTH & CRYPTOGRAPHIC COOKIE PROTECTION LAYERS
// ======================================================

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenRefreshCoordinator>();

builder.Services.AddAntiforgery();


// ======================================================
// 3. VALIDATION
// ======================================================
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// ======================================================
// 4. UI FRAMEWORKS & ENGINE CONFIGURATIONS
// ======================================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(o => o.DetailedErrors = true);

builder.Services.AddFluentUIComponents();
builder.Services.AddRadzenComponents();

builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.DialogService>();

builder.Services.AddBrowserStorage();

// ======================================================
// 5. APPLICATION STATE ARCHITECTURE CONTAINERS
// ======================================================
builder.Services.AddScoped<DashboardState>();
builder.Services.AddScoped<WorkspaceState>();
builder.Services.AddScoped<EmailContextState>();
builder.Services.AddScoped<UIState>();
// 🚀 FIXED: Registers the context builder right into the active UI project container where the Dashboard lives
builder.Services.AddScoped<EmailContextBuilder>();

builder.Services.AddScoped<SageStateService>();

// Ensure AppStateContainer is registered as scoped so it is the single source-of-truth for circuit session state
builder.Services.AddScoped<AppStateContainer>();
builder.Services.AddScoped<EventBus>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueueService>();
builder.Services.AddScoped<ISageSyncJobs, SageSyncJobs>();

builder.Services.AddScoped<OutlookStateContainer>();

builder.Services.AddScoped<IUserContextService, UserContextService>();
// ======================================================
// 6. BACKEND REST API NETWORK CHANNELS
// ======================================================
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7123");
});

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// ======================================================
// 7. PRESENTATION RUNTIME CORE SERVICES
// ======================================================
builder.Services.AddScoped<DashboardUIService>();
builder.Services.AddScoped<EmailContextUIService>();
builder.Services.AddScoped<QuickActionUIService>();
builder.Services.AddScoped<BusinessPartnerUIService>();
builder.Services.AddScoped<OrdersUIService>();
builder.Services.AddScoped<TasksUIService>();
builder.Services.AddSingleton<ModalService>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<EmailSyncService>();
builder.Services.AddScoped<ActivityUIService>();
builder.Services.AddScoped<AttachmentUIService>();
builder.Services.AddScoped<SettingsUIService>();
builder.Services.AddScoped<AdminDashboardUIService>();
builder.Services.AddScoped<KnowledgeUIService>();
builder.Services.AddScoped<EmailOrchestrationService>();

// ======================================================
// 8. EMAIL ENRICHMENT & API REDIRECTS
// ======================================================
builder.Services.AddScoped<EmailEnrichmentService>();

builder.Services.AddScoped<MockUnifiedService>();

builder.Services.AddScoped<IActivityService>(sp => sp.GetRequiredService<MockUnifiedService>());
builder.Services.AddScoped<IEmailService>(sp => sp.GetRequiredService<MockUnifiedService>());
builder.Services.AddScoped<IKnowledgeService>(sp => sp.GetRequiredService<MockUnifiedService>());
builder.Services.AddScoped<IInvoiceService>(sp => sp.GetRequiredService<MockUnifiedService>());
builder.Services.AddScoped<ISalesService>(sp => sp.GetRequiredService<MockUnifiedService>());
builder.Services.AddScoped<IBusinessPartnerService>(sp => sp.GetRequiredService<MockUnifiedService>());

builder.Services.AddScoped<ITaskService>(sp => sp.GetRequiredService<MockUnifiedService>());

builder.Services.AddScoped<IAuditLogService, MockAuditService>();
builder.Services.AddScoped<ISystemHealthService, MockSystemHealthService>();

// ======================================================
// 9. TOAST MESSAGING NOTIFICATIONS
// ======================================================
builder.Services.AddSingleton<ToastService>();

builder.Services.AddSingleton<IToastUIService>(sp =>
    sp.GetRequiredService<ToastService>());
// ======================================================
// 11. BUILD WEB APPLICATION HOST ENGINE
// ======================================================
var app = builder.Build();

// ======================================================
// 12. RUNTIME MIDDLEWARE SECURITY PIPELINE
// ======================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
// Temporary security headers middleware for local verification (adds CSP including Outlook origins)
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    if (context.Request.IsHttps)
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");
    }

    string csp = "default-src 'self'; " +
                 "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://appsforoffice.microsoft.com; " +
                 "style-src 'self' 'unsafe-inline'; " +
                 "img-src 'self' data: https:; " +
                 "font-src 'self' https: data:; " +
                 "connect-src 'self' https: wss:; " +
                 "frame-ancestors 'self' " +
                 "https://outlook.office.com " +
                 "https://outlook.office365.com " +
                 "https://outlook.live.com " +
                 "https://appsforoffice.microsoft.com " +
                 "https://office.com " +
                 "https://office365.com " +
                 "https://localhost:7173 " +
                 "https://localhost:7123;";

    context.Response.Headers.Append("Content-Security-Policy", csp);

    await next();
});

app.UseStaticFiles();

// Ensure routing and antiforgery middleware are registered for endpoints that require antiforgery tokens
app.UseRouting();


// Adds the antiforgery middleware so endpoints with antiforgery metadata are validated
app.UseAntiforgery();

// ======================================================
// 13. DATA INTERACTIVE ROUTING TARGET MAPS
// ======================================================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Temporary endpoint to dump request/response headers for easy verification while debugging Outlook embedding.
app.MapGet("/debug/headers", (HttpContext context) =>
{
    var responseHeaders = context.Response.Headers.ToDictionary(kvp => kvp.Key, kvp => string.Join(";", kvp.Value.Select(v => v ?? string.Empty)));
    var requestHeaders = context.Request.Headers.ToDictionary(kvp => kvp.Key, kvp => string.Join(";", kvp.Value.Select(v => v ?? string.Empty)));

    return Results.Json(new { requestHeaders, responseHeaders });
});

// ======================================================
// 14. EXECUTE APPLICATION CIRCUIT LIFE CYCLES
// ======================================================
app.Run();