using FluentValidation;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.Services;
// REMOVED INAPPROPRIATE INFRASTRUCTURE USINGS HERE
using OperationalWorkspaceShared.Validators;
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

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
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
builder.Services.AddScoped<SageStateService>();

builder.Services.AddScoped<AppStateContainer>();
builder.Services.AddScoped<EventBus>();

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
builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<EmailSyncService>();
builder.Services.AddScoped<ActivityUIService>();
builder.Services.AddScoped<AttachmentUIService>();
builder.Services.AddScoped<SettingsUIService>();
builder.Services.AddScoped<AdminDashboardUIService>();
builder.Services.AddScoped<KnowledgeUIService>();

// ======================================================
// 8. EMAIL ENRICHMENT & API REDIRECTS (Decoupled From Mock Infrastructure)
// ======================================================
builder.Services.AddScoped<EmailEnrichmentService>();

// CRITICAL FIX: The UI now depends on an API client proxy or application container rather than raw mock files.
// For now, if you are calling these over HTTP, we point them to application handlers or an API client.
// If you still need runtime stubs for the frontend to boot, implement them inside the Application layer, not Infrastructure.

// ======================================================
// 9. TOAST MESSAGING NOTIFICATIONS
// ======================================================
builder.Services.AddScoped<IToastUIService, ToastService>();
builder.Services.AddScoped<ToastService>();

// ======================================================
// 10. COMPLIANCE AUDIT TRACING SERVICE LOGGERS
// ======================================================
// REMOVED: Direct injection of MockAuditService. 
// Your UI components should log actions by executing an API call or through an application wrapper.

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
app.UseStaticFiles();

app.UseAntiforgery();

// ======================================================
// 13. DATA INTERACTIVE ROUTING TARGET MAPS
// ======================================================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ======================================================
// 14. EXECUTE APPLICATION CIRCUIT LIFE CYCLES
// ======================================================
app.Run();
