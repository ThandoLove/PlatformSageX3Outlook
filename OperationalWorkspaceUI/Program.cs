// CODE START

using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services; // IMPORTANT
using OperationalWorkspaceUI.Components;

using Majorsoft.Blazor.Extensions.BrowserStorage;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Actions;
using OperationalWorkspaceUI.UIServices.DashboardUI;
using OperationalWorkspaceUI.UIServices.EmailService;
using OperationalWorkspaceUI.UIServices.System;
using OperationalWorkspaceUI.UIServices.Workspace;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// ------------------ 1. SYSTEM ------------------
builder.Services.AddDistributedMemoryCache();

// ------------------ 2. UI ------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.DialogService>();
// Add this to your builder.Services
builder.Services.AddBrowserStorage();

// ------------------ 3. STATE ------------------
builder.Services.AddScoped<DashboardState>();
builder.Services.AddScoped<WorkspaceState>();
builder.Services.AddScoped<EmailContextState>();
builder.Services.AddScoped<UIState>();

// ------------------ 4. API CLIENT ------------------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7123"
    );
});

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// ------------------ 5. UI SERVICES ------------------
builder.Services.AddScoped<DashboardUIService>();
builder.Services.AddScoped<EmailContextUIService>();
builder.Services.AddScoped<QuickActionUIService>();
builder.Services.AddScoped<BusinessPartnerUIService>();
builder.Services.AddScoped<OrdersUIService>();
builder.Services.AddScoped<TasksUIService>();
builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailSyncService>();

// ------------------ 6. 🔥 BACKEND SERVICES (FIX) ------------------
builder.Services.AddScoped<IActivityService, MockUnifiedService>();
builder.Services.AddScoped<IEmailService, MockUnifiedService>();
builder.Services.AddScoped<IKnowledgeService, MockUnifiedService>();
builder.Services.AddScoped<ISalesService, MockUnifiedService>();
builder.Services.AddScoped<IBusinessPartnerService, MockUnifiedService>(); // 🔥 THIS FIXES YOUR ERROR
builder.Services.AddScoped<IInventoryService, MockUnifiedService>();
builder.Services.AddScoped<ITaskService, MockUnifiedService>();
builder.Services.AddScoped<IInvoiceService, MockUnifiedService>();

var app = builder.Build();

// ------------------ 7. PIPELINE ------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();





// CODE END