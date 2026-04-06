using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components; // Added for FluentUI
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceUI.Components;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Actions;
using OperationalWorkspaceUI.UIServices.EmailService;
using OperationalWorkspaceUI.UIServices.System;
using OperationalWorkspaceUI.UIServices.Workspace;
using OperationalWorkspaceUI.UIServices.DashboardUI;

// FIX: Check your Infrastructure project for these exact namespaces
using OperationalWorkspaceInfrastructure;
// If 'DependencyInjection' or 'Persistence' still show red, 
// check the folder names in your Infrastructure project.

var builder = WebApplication.CreateBuilder(args);

// ------------------ BLAZOR + FLUENT UI ------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents(); // Critical for your Visual Goal

// ------------------ STATE ------------------
builder.Services.AddScoped<DashboardState>();
builder.Services.AddScoped<WorkspaceState>();
builder.Services.AddScoped<EmailContextState>();
builder.Services.AddScoped<UIState>();

// ------------------ HTTP CLIENT ------------------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7123");
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// ------------------ UI SERVICES ------------------
builder.Services.AddScoped<DashboardUIService>();
builder.Services.AddScoped<EmailContextUIService>();
builder.Services.AddScoped<QuickActionUIService>();
builder.Services.AddScoped<BusinessPartnerUIService>();
builder.Services.AddScoped<OrdersUIService>();
builder.Services.AddScoped<TasksUIService>();
builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<OperationalWorkspaceUI.UIServices.System.AuthService>();
builder.Services.AddScoped<OperationalWorkspaceUI.UIServices.EmailService.EmailSyncService>();

// ------------------ APPLICATION & INFRASTRUCTURE ------------------
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IKnowledgeService, KnowledgeService>();

// FIX: Ensure these classes exist in your Infrastructure Project
// If they are missing, you must create the .cs files first!
//builder.Services.AddScoped<IEmailRepository, EmailRepository>(); 

//builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// ------------------ PIPELINE ------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Use this instead of MapStaticAssets for standard Blazor
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
