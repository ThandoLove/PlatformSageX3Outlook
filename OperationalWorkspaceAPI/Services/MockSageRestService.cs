using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceAPI.Services;

public class MockSageRestService : ISageRestService
{
    public Task<T?> GetAsync<T>(string entity, string id)
    {
        return Task.FromResult<T?>(default);
    }

    public Task<bool> PostAsync<T>(string entity, T data)
    {
        return Task.FromResult(true);
    }

    // FIX: Implement the missing list method for customers
    public Task<dynamic> GetCustomersAsync()
    {
        // Return a mock list so your Blazor table isn't empty during testing
        var mockCustomers = new[] {
            new { BPCNUM_0 = "CUST001", BPCNAM_0 = "Mock Customer One" },
            new { BPCNUM_0 = "CUST002", BPCNAM_0 = "Mock Customer Two" }
        };
        return Task.FromResult<dynamic>(mockCustomers);
    }

    // FIX: Implement the missing detail method for partners
    public Task<dynamic> GetPartnerByIdAsync(string id)
    {
        return Task.FromResult<dynamic>(new
        {
            BPRNUM_0 = id,
            BPRNAM_0 = "Mock Partner Detail"
        });
    }
}
