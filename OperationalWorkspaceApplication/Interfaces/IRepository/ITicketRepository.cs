using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface ITicketRepository
{
    Task<bool> CreateTicketAsync(TicketRequest request);
}

public class TicketRepo : ITicketRepository
{
    private readonly ISageRestService _sageService;

    public TicketRepo(ISageRestService sageService)
    {
        _sageService = sageService;
    }

    public async Task<bool> CreateTicketAsync(TicketRequest request)
    {
        // "ITN" is the Sage X3 internal code for Tickets/Incidents
        return await _sageService.PostAsync<TicketRequest>("ITN", request);
    }
}
