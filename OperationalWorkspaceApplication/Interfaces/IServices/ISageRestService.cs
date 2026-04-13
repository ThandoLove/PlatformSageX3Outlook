using System;
using System.Collections.Generic;
using System.Text;


namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface ISageRestService
{
    // Generic GET: Fetches any entity (Customer, Order, Ticket) by ID
    Task<T?> GetAsync<T>(string entity, string id);
    Task<dynamic> GetCustomersAsync();
    Task<dynamic> GetPartnerByIdAsync(string id);

    // Generic POST: Sends data to Sage X3 to create a record
    Task<bool> PostAsync<T>(string entity, T data);
}
