using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceUI.State;

namespace OperationalWorkspaceApplication.Services;

public class EmailEnrichmentService
{
    private readonly AppStateContainer _appState;
    private readonly IBusinessPartnerService _bpService;
    private readonly IOrderService _orderService;
    private readonly ITaskService _taskService;

    private string? _lastEmailId;

    public EmailEnrichmentService(
        AppStateContainer appState,
        IBusinessPartnerService bpService,
        IOrderService orderService,
        ITaskService taskService)
    {
        _appState = appState;
        _bpService = bpService;
        _orderService = orderService;
        _taskService = taskService;
    }

    public async Task EnrichAsync(EmailInsightDto email)
    {
        if (email == null)
            return;

        if (_lastEmailId == email.Id.ToString())
            return;

        _lastEmailId = email.Id.ToString();

        _appState.SetBusy(true);

        try
        {
            _appState.SetCurrentEmail(email);

            var clientTask =
                _bpService.GetPartnerByEmailAsync(email.SenderEmail);

            var ordersTask =
                _orderService.GetOpenOrdersAsync();

            var tasksTask =
                _taskService.GetAllTasksAsync();

            await Task.WhenAll(clientTask, ordersTask, tasksTask);

            _appState.SetMatchedClient(await clientTask);
            _appState.SetLinkedOrders(await ordersTask);
            _appState.SetLinkedTasks(await tasksTask);
        }
        finally
        {
            _appState.SetBusy(false);
        }
    }
}