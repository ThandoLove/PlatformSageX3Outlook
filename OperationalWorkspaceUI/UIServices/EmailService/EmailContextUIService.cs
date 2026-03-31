using System;
using System.Collections.Generic;
using System.Linq; // This will light up once the code below is pasted
using System.Threading.Tasks;
using OperationalWorkspaceUI.State;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceUI.UIServices.EmailService;

public class EmailContextUIService
{
    private readonly IEmailService _emailService;

    public EmailContextUIService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task LoadEmailContextAsync(string emailId, EmailContextState state, WorkspaceState workspaceState)
    {
        // 1. Fetch email
        state.CurrentEmail = await _emailService.GetEmailByIdAsync(emailId);

        // 2. Fetch the "Open" orders (Returns List<OpenOrderDto>)
        var openOrders = await _emailService.GetLinkedOrdersAsync(emailId);

        // 3. FIX: Manually map OpenOrderDto to OrderDto to satisfy the State property type
        // This is what makes "using System.Linq" active
        state.LinkedOrders = openOrders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ClientId = Guid.Empty,
            Description = "Linked via email context"
        }).ToList();

        // 4. Preload linked tasks
        state.LinkedTasks = await _emailService.GetLinkedTasksAsync(emailId);

        // 5. Match clients automatically
        if (state.CurrentEmail != null && workspaceState.Clients != null)
        {
            state.MatchedClient = workspaceState.Clients
                .FirstOrDefault(c => c.Email == state.CurrentEmail.From);
        }
    }
}
