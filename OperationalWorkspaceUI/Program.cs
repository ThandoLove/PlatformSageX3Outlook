using FluentValidation;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceInfrastructure.ExternalServices.SageX3.Mock;
using OperationalWorkspaceInfrastructure.Services;
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
using ToastService = OperationalWorkspaceUI.UIServices.ToastUIService.ToastService;

var builder = WebApplication.CreateBuilder(args);


// ======================================================
// 1. SYSTEM SERVICES
// ======================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();


// ======================================================
// 2. AUTHENTICATION & AUTHORIZATION
// ======================================================

// REQUIRED FOR BLAZOR AUTHORIZATION
builder.Services.AddAuthorizationCore();

// REQUIRED FOR <CascadingAuthenticationState>
builder.Services.AddCascadingAuthenticationState();

// CUSTOM AUTH STATE PROVIDER
builder.Services.AddScoped<
    AuthenticationStateProvider,
    CustomAuthenticationStateProvider>();

// AUTH SERVICE
builder.Services.AddScoped<AuthService>();

// TOKEN REFRESH COORDINATOR
builder.Services.AddScoped<TokenRefreshCoordinator>();


// ======================================================
// 3. VALIDATION
// ======================================================

builder.Services.AddValidatorsFromAssemblyContaining<
    LoginRequestValidator>();


// ======================================================
// 4. UI FRAMEWORKS
// ======================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddFluentUIComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddScoped<Radzen.NotificationService>();

builder.Services.AddScoped<Radzen.DialogService>();

builder.Services.AddBrowserStorage();


// ======================================================
// 5. APPLICATION STATE
// ======================================================

// EXISTING STATE
builder.Services.AddScoped<DashboardState>();

builder.Services.AddScoped<WorkspaceState>();

builder.Services.AddScoped<EmailContextState>();

builder.Services.AddScoped<UIState>();

builder.Services.AddScoped<SageStateService>();

// NEW CENTRALIZED STATE
builder.Services.AddScoped<AppStateContainer>();

// EVENT BUS
builder.Services.AddScoped<EventBus>();

builder.Services.AddScoped<
    IUserContextService,
    UserContextService>();


// ======================================================
// 6. API CLIENT
// ======================================================

builder.Services.AddHttpClient(
    "ApiClient",
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration["ApiBaseUrl"]
                ?? "https://localhost:7123");
    });

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
        .CreateClient("ApiClient"));


// ======================================================
// 7. UI SERVICES
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
// 8. TOAST SERVICES
// ======================================================

builder.Services.AddScoped<
    IToastUIService,
    ToastService>();

builder.Services.AddScoped<ToastService>();


// ======================================================
// 9. BACKEND MOCK SERVICES
// ======================================================

builder.Services.AddScoped<
    IActivityService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IEmailService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IKnowledgeService,
    MockUnifiedService>();

builder.Services.AddScoped<
    ISalesService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IBusinessPartnerService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IInventoryService,
    MockUnifiedService>();

builder.Services.AddScoped<
    ITaskService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IInvoiceService,
    MockUnifiedService>();

builder.Services.AddScoped<
    IAuditLogService,
    MockAuditService>();


// ======================================================
// 10. BUILD APPLICATION
// ======================================================

var app = builder.Build();


// ======================================================
// 11. PIPELINE
// ======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();


// ======================================================
// 12. MAP RAZOR COMPONENTS
// ======================================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// ======================================================
// 13. RUN APPLICATION
// ======================================================

app.Run();