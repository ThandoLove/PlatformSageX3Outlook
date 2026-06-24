
namespace OperationalWorkspaceUI.Models.Constant
{
    public static class AppRoutes
    {
        // =========================================
        // CORE
        // =========================================
        public const string Home = "/";

        public const string Welcome = "/welcome";
        public const string Dashboard = "/dashboard";
        public const string Settings = "/settings";

        // =========================================
        // CRM / WORKSPACE
        // =========================================

        public const string BusinessPartners = "/contacts";
        public const string Tickets = "/tickets";
        public const string Activity = "/activities";
        public const string Knowledge = "/knowledge-page";
        public const string Tasks = "/tasks";
        public const string Attachments = "/attachments";
        public const string Orders = "/orders";
        public const string Quotes = "/quotes";

        // =========================================
        // ADMIN
        // =========================================

        public const string Administration = "/admin-dashboard";
        public const string AdministrationTasks = "/admin-tasks";

        public const string UserManagement = "/admin/users";
        public const string RoleManagement = "/admin/roles";
        public const string SystemHealth = "/admin/system-health";
        public const string AuditLogs = "/admin/audit-logs";
        public const string ERPConnections = "/admin/erp-connections";

        // =========================================
        // AUTH
        // =========================================

        public const string Login = "/login";


       
    }
}