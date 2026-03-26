
using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Services;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceUI.UIServices.Actions;

public class QuickActionUIService
{
    private readonly IOrderService _orderService;
    private readonly ITaskService _taskService;

    public QuickActionUIService(IOrderService orderService, ITaskService taskService)
    {
        _orderService = orderService;
        _taskService = taskService;
    }

    public async Task<OrderDTO> CreateOrderFromEmailAsync(EmailDTO email)
    {
        var order = new OrderDTO
        {
            ClientId = email.ClientId,
            OrderDate = DateTime.UtcNow,
            Description = $"Order from email {email.Subject}"
        };
        return await _orderService.CreateOrderAsync(order);
    }

    public async Task<TaskDTO> CreateTaskFromEmailAsync(EmailDTO email)
    {
        var task = new TaskDTO
        {
            Title = $"Follow-up: {email.Subject}",
            Description = email.Body,
            AssignedToId = email.AssignedUserId,
            DueDate = DateTime.UtcNow.AddDays(3)
        };
        return await _taskService.CreateTaskAsync(task);
    }
}