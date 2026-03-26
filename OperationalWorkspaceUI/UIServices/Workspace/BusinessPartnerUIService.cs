
using System.Collections.Generic;

using System.Threading.Tasks;

namespace OperationalWorkspaceUI.UIServices.Workspace
{
    public class BusinessPartnerUIService
    {
        private readonly List<string> _partners = new();

        public Task<List<string>> GetPartnersAsync()
        {
            // Replace with API call
            return Task.FromResult(_partners);
        }

        public Task AddPartnerAsync(string partner)
        {
            _partners.Add(partner);
            return Task.CompletedTask;
        }
    }
}