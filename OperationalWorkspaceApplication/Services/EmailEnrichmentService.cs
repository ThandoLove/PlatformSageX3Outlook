using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceApplication.Services;

public class EmailEnrichmentService
{
    private readonly AppStateContainer _appState;
    private readonly IBusinessPartnerService _bpService;
    private readonly ITaskService _taskService;

    public EmailEnrichmentService(
        AppStateContainer appState,
        IBusinessPartnerService bpService,
        ITaskService taskService)
    {
        _appState = appState;
        _bpService = bpService;
        _taskService = taskService;
    }

    public async Task EnrichAsync(EmailInsightDto email)
    {
        if (email == null) return;

        // EMAIL
        _appState.SetCurrentEmail(email);

        // CRM enrichment
        if (!string.IsNullOrWhiteSpace(email.SenderEmail))
        {
            var partner = await _bpService.GetPartnerByEmailAsync(email.SenderEmail);

            // ✅ FIXED LINE
            _appState.SetMatchedClient(partner);
        }

        await Task.CompletedTask;
    }
}