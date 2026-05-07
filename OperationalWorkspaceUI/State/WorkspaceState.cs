using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceUI.State
{
    public class WorkspaceState
    {
        // 1. DATA PROPERTIES
        public List<ClientDto> Clients { get; set; } = new();
        public List<ContactCreateDto> Contacts { get; set; } = new();
        public List<OrderDto> Orders { get; set; } = new();
        public List<TaskDto> Tasks { get; set; } = new();
        public List<ActivityDto> ActivityLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<TicketDto> Tickets { get; set; } = new(); // Added Tickets list
        public List<UserDto> Users { get; set; } = new();
        public List<string> Knowledge { get; set; } = new();
        public List<OpenOrderDto> Quotes { get; set; } = new();
        public EmailInsightDto? SelectedEmail { get; set; }

        // GLOBAL FLAGS FOR DASHBOARD PREVIEWS
        public bool IsUploading { get; set; }
        public bool IsSendingKnowledge { get; set; }

        // --- CONSTRUCTOR: PRE-FILLS DATA ON START ---
        public WorkspaceState()
        {
            // Seed Initial Attachments
            Attachments = new List<AttachmentDto> {
                new(Guid.NewGuid(), "QTE-7789_Proposal.pdf", "Proposal", 1240000, "#", "Quote - QTE-7789", DateTime.Now),
                new(Guid.NewGuid(), "INV-2024-00567_Invoice.pdf", "Invoice", 960000, "#", "Invoice - INV-2024-00567", DateTime.Now)
            };

            // Seed Initial Knowledge Base
            KnowledgeBase = new List<KnowledgeDto> {
                new KnowledgeDto(Guid.NewGuid(), "Pricing Guidelines 2024", "Sage X3 pricing tiered structure for 2024.", "Pricing Guidelines", "#", "Sales"),
                new KnowledgeDto(Guid.NewGuid(), "Product Installation Guide", "Step-by-step server configuration...", "Product Documentation", "#", "IT"),
                new KnowledgeDto(Guid.NewGuid(), "Frequently Asked Questions", "Top user permission answers...", "FAQ", "#", "Support"),
                new KnowledgeDto(Guid.NewGuid(), "How to Create a Sales Order", "Full O-WS interface instructions...", "How To Guides", "#", "Guides")
            };

            // Seed Initial Tickets
            Tickets = new List<TicketDto> {
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1001", Title = "Sage X3 Connection Timeout", CustomerName = "Tech Innovations Inc.", Priority = "High", Status = "Open", CreatedAt = DateTime.Now.AddDays(-2) },
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1002", Title = "Invoice Export Error", CustomerName = "Global Systems", Priority = "Medium", Status = "In Progress", CreatedAt = DateTime.Now.AddDays(-1) },
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1003", Title = "New User Onboarding", CustomerName = "Brightwave", Priority = "Low", Status = "Resolved", CreatedAt = DateTime.Now }
            };

            // Start loading tasks automatically
            _ = LoadTasksAsync();
        }

        // 2. STATE NOTIFICATION (Broadcast)
        public event Action? OnChange;
        public void Notify() => OnChange?.Invoke();

        public void SetUploading(bool status)
        {
            IsUploading = status;
            Notify();
        }

        public void SetSendingKnowledge(bool status)
        {
            IsSendingKnowledge = status;
            Notify();
        }

        // 3. LOGGING LOGIC
        public void LogActivity(string title, string action, string user = "Current User")
        {
            var newLog = new ActivityDto { Id = Guid.NewGuid(), Title = title, Action = action, CreatedBy = user, Timestamp = DateTime.Now };
            ActivityLogs.Insert(0, newLog);
            Notify();
        }

        // 4. DATA RELOAD METHODS
        public async Task LoadTasksAsync()
        {
            await Task.Delay(100);
            Tasks = new List<TaskDto>
            {
                new TaskDto { Id = Guid.NewGuid(), Title = "Inquiry about Product XYZ", CompanyName = "Tech Innovations Inc.", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Setup Sage X3 Connector", CompanyName = "Sage Internal", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Forecast Q3", CompanyName = "Global Systems", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Verify Invoice #8821", CompanyName = "Brightwave", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Submit Expense Report", CompanyName = "Finance Dept", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                new TaskDto { Id = Guid.NewGuid(), Title = "Daily Team Sync", CompanyName = "Operational Team", Priority = TaskPriority.Medium, DueDate = DateTime.Today, Status = TaskStatus.Open }
            };
            Notify();
        }

        public void ReloadKnowledge(List<KnowledgeDto> articles) { KnowledgeBase = articles; Notify(); }
        public void ReloadContacts(List<ContactCreateDto> contacts) { Contacts = contacts; Notify(); }
        public void ReloadClients() { Notify(); }
        public void ReloadOrders() { Notify(); }
        public void ReloadTasks() { Notify(); }
    }
}
