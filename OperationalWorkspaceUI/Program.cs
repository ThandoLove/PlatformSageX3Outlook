// CODE START: Program.cs (CORRECT FOR YOUR PROJECT)


using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceUI.Components;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceUI.UIServices.Actions;
using OperationalWorkspaceUI.UIServices.EmailService;
using OperationalWorkspaceUI.UIServices.System;
using OperationalWorkspaceUI.UIServices.Workspace;
using OperationalWorkspace.UIServices.DashboardUI;

var builder = WebApplication.CreateBuilder(args);

// ------------------ BLAZOR SERVER ------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ------------------ STATE ------------------
builder.Services.AddScoped<DashboardState>();
builder.Services.AddScoped<WorkspaceState>();
builder.Services.AddScoped<EmailContextState>();
builder.Services.AddScoped<UIState>();

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

// ------------------ APPLICATION LAYER ------------------
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IKnowledgeService, KnowledgeService>();

var app = builder.Build();

// ------------------ PIPELINE ------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

// Static files (CSS/JS)
app.MapStaticAssets();

// Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// CODE END