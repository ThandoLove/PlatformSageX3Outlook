using Microsoft.AspNetCore.Components;
using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Orchestration;
using OperationalWorkspaceUI.State;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceUI.Pages.Demo;

public partial class EmailSimulator : ComponentBase
{
    // =========================================================
    // INJECTED SERVICES & STATE CONTAINERS
    // =========================================================
    [Inject] public IEmailService EmailService { get; set; } = null!;
    [Inject] public IBusinessPartnerService BPService { get; set; } = null!;
    [Inject] public IActivityService ActivityService { get; set; } = null!;
    [Inject] public ITaskService TaskService { get; set; } = null!;
    [Inject] public AppStateContainer AppState { get; set; } = null!;
    [Inject] public DashboardState DashboardState { get; set; } = null!;
    [Inject] public EmailOrchestrationService Orchestrator { get; set; } = null!;

    // =========================================================
    // PUBLIC BOUND FIELDS FOR RAZOR MARKUP VISIBILITY
    // =========================================================
    public List<EmailInsightDto> Emails { get; set; } = new();
    public EmailInsightDto? CurrentEmail { get; set; }
    public BusinessPartnerSnapshotDto? CurrentCustomer { get; set; }

    protected override void OnInitialized()
    {
        // =========================
        // MOCK EMAILS (DEMO READY)
        // =========================
        Emails = new List<EmailInsightDto>
        {
            new()
            {
                MessageId = "1",
                Subject = "PO Delay - Order 7741",
                From = "john@democorp.com",
                Message = "Please update delivery timeline for PO 7741"
            },
            new()
            {
                MessageId = "2",
                Subject = "Invoice overdue INV-9001",
                From = "finance@abc.com",
                Message = "Invoice INV-9001 is overdue"
            },
            new()
            {
                MessageId = "3",
                Subject = "New Support Request",
                From = "support@contoso.com",
                Message = "System error on order processing"
            }
        };
    }

    // ✅ FIXED: Changed to asynchronous signature to safely invoke background data orchestration loops
    public async Task SelectEmail(EmailInsightDto email)
    {
        CurrentEmail = email;

        // Dispatches processing straight down your multi-service pipeline execution loop
        await Orchestrator.ExecuteAsync(email);

        // Instantly captures updated state variables from the container engine
        CurrentCustomer = AppState.MatchedClient;

        StateHasChanged();
    }

    public async Task LinkCustomer()
    {
        var email = CurrentEmail;
        if (email == null) return;

        AppState.AddAutomation($"Linking customer from: {email.From}");

        CurrentCustomer = await BPService.GetPartnerByEmailAsync(email.From);

        AppState.SetMatchedClient(CurrentCustomer);
    }

    public async Task CreateActivity()
    {
        if (CurrentCustomer == null || CurrentEmail == null) return;

        var activityDto = new CreateActivityDto(
            $"Email: {CurrentEmail.Subject}",
            CurrentEmail.Message ?? string.Empty,
            "Email",
            CurrentCustomer.Id
        );

        await ActivityService.CreateAsync(activityDto, "Demo User");

        AppState.AddAutomation($"Activity created: {CurrentEmail?.Subject}");
    }

    public async Task CreateTask()
    {
        if (CurrentEmail == null) return;

        await TaskService.CreateAsync(
            new CreateTaskRequest
            {
                Title = CurrentEmail.Subject,
                Description = CurrentEmail.Message
            },
            default
        );

        AppState.AddAutomation($"Task created from email");
    }

    public async Task OpenCustomer()
    {
        if (CurrentCustomer == null) return;

        await DashboardState.LoadOrderDetailsAsync(null);
    }

    public void ClearLog()
    {
        AppState.ClearAutomation();
    }
}
