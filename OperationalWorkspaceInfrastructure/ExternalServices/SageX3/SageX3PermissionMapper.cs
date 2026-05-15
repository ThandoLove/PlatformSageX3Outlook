using System;
using System.Collections.Generic;
using System.Text;


namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public static class SageX3PermissionMapper
{
    public static List<string> MapRole(string role)
    {
        return role switch
        {
            "Admin" => new List<string>
            {
                "CONTACT_CREATE",
                "CONTACT_EDIT",
                "CONTACT_DELETE",
                "OPPORTUNITY_CREATE",
                "INVOICE_APPROVE"
            },

            "Sales" => new List<string>
            {
                "CONTACT_CREATE",
                "CONTACT_EDIT",
                "OPPORTUNITY_CREATE"
            },

            "Support" => new List<string>
            {
                "CONTACT_VIEW"
            },

            _ => new List<string>()
        };
    }
}