using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;    


namespace OperationalWorkspaceInfrastructure.Persistence.Repositories
{
  

    public class TicketRepository : ITicketRepository
    {
        private readonly ISageRestService _sageService;

        public TicketRepository(ISageRestService sageService)
        {
            _sageService = sageService;
        }

        public async Task<TicketResponse?> GetTicketByIdAsync(string ticketNumber)
        {
            // "ITN" is the Sage X3 code for the Service Incident/Ticket entity
            var entity = await _sageService.GetAsync<SageTicket>("ITN", ticketNumber);

            if (entity == null) return null;

            return new TicketResponse
            {
                TicketNumber = entity.ITNNUM_0,
                Description = entity.ITNDES_0,
                CustomerCode = entity.BPCNUM_0,
                Status = entity.ITNSTA_0
            };
        }

        public async Task<bool> CreateTicketAsync(TicketRequest request)
        {
            // Sends the request to the Sage 'Create' endpoint for tickets
            return await _sageService.PostAsync<TicketRequest>("ITN", request);
        }
    }

}
