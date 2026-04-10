// CODE START

using Microsoft.FluentUI.AspNetCore.Components;
using OperationalWorkspaceUI.Components;
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

// ------------------ 5. UI SERVICES ONLY ------------------
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

var app = builder.Build();

// ------------------ 6. PIPELINE ------------------
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