using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspaceInfrastructure.Persistence;
using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceInfrastructure.DependencyInjection
{
    public static class InfrastructureSeeder
    {
        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();

            // Only seed if DB empty
            if (db.BusinessPartners.Any()) return;

            var bp = new BusinessPartner("C1000", "Tech Innovations Inc.", 10000m)
            {
                ContactName = "Sarah Johnson",
                ContactEmail = "sarah.johnson@techinnovate.com",
                City = "Los Angeles",
                State = "CA",
                SalesRepName = "John Smith",
                IsLead = false
            };

            db.BusinessPartners.Add(bp);

            db.Emails.Add(new Email { MessageId = Guid.NewGuid().ToString(), Subject = "Quote Request", From = "sarah.johnson@techinnovate.com", ReceivedAt = DateTime.UtcNow });

            db.SaveChanges();
        }
    }
}
