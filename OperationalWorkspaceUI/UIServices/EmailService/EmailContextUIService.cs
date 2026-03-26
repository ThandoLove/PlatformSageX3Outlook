
using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Services;

namespace OperationalWorkspaceUI.UIServices.EmailService;

public class EmailContextUIService
{
    private readonly IEmailService _emailService; // Application layer service

    public EmailContextUIService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task LoadEmailContextAsync(string emailId, EmailContextState state, WorkspaceState workspaceState)
    {
        // Fetch email
        state.CurrentEmail = await _emailService.GetEmailByIdAsync(emailId);

        // Preload linked orders / tasks from Application layer
        state.LinkedOrders = await _emailService.GetLinkedOrdersAsync(emailId);
        state.LinkedTasks = await _emailService.GetLinkedTasksAsync(emailId);

        // Optionally match clients automatically
        state.MatchedClient = workspaceState.Clients
            .FirstOrDefault(c => c.Email == state.CurrentEmail?.From);
    }
}