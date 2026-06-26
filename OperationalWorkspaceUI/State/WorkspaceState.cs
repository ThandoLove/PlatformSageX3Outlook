using OperationalWorkspace.Domain.Enums;
using OperationalWorkspaceApplication.DTOs;

using TaskStatus = OperationalWorkspace.Domain.Enums.TaskStatus;

namespace OperationalWorkspaceUI.State
{
    public class WorkspaceState : IDisposable
    {
        // ======================================================
        // DATA PROPERTIES
        // ======================================================
        public List<ClientDto> Clients { get; set; } = new();
        public List<ContactCreateDto> Contacts { get; set; } = new();
        
        public List<TaskDto> Tasks { get; set; } = new();
        public List<ActivityDto> ActivityLogs { get; set; } = new();
        public List<KnowledgeDto> KnowledgeBase { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<TicketDto> Tickets { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<string> Knowledge { get; set; } = new();
        public List<OpenOrderDto> Quotes { get; set; } = new();
        public EmailInsightDto? SelectedEmail { get; set; }

        public List<GlobalActivityItem> SharedSystemLogs { get; private set; } = new();

        // ======================================================
        // GLOBAL FLAGS
        // ======================================================
        public bool IsUploading { get; set; }
        public bool IsSendingKnowledge { get; set; }
        public bool IsLoadingTasks { get; set; }
        // ======================================================
        // CONSTRUCTOR
        // ======================================================
        public WorkspaceState()
        {
            // 🚀 FIXED: Cleared hardcoded list entries to prevent overriding your mock directory repository streams
            Attachments = new List<AttachmentDto>();

            KnowledgeBase = new List<KnowledgeDto>
            {
                new KnowledgeDto(Guid.NewGuid(), "Pricing Guidelines 2024", "Sage X3 pricing tiered structure for 2024.", "Pricing Guidelines", "#", "Sales", 0),
                new KnowledgeDto(Guid.NewGuid(), "Product Installation Guide", "Step-by-step server configuration...", "Product Documentation", "#", "IT", 0),
                new KnowledgeDto(Guid.NewGuid(), "Frequently Asked Questions", "Top user permission answers...", "FAQ", "#", "Support", 0),
                new KnowledgeDto(Guid.NewGuid(), "How to Create a Sales Order", "Full O-WS interface instructions...", "How To Guides", "#", "Guides", 0)
            };

            Tickets = new List<TicketDto>
            {
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1001", Title = "Sage X3 Connection Timeout", CustomerName = "Tech Innovations Inc.", Priority = "High", Status = "Open", CreatedAt = DateTime.Now.AddDays(-2) },
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1002", Title = "Invoice Export Error", CustomerName = "Global Systems", Priority = "Medium", Status = "In Progress", CreatedAt = DateTime.Now.AddDays(-1) },
                new TicketDto { Id = Guid.NewGuid(), TicketNumber = "TKT-1003", Title = "New User Onboarding", CustomerName = "Brightwave", Priority = "Low", Status = "Resolved", CreatedAt = DateTime.Now }
            };

            _ = LoadTasksAsync();
        }
        // ======================================================
        // EVENTS
        // ======================================================
        public event Action? OnChange;
        public void Notify()
        {
            OnChange?.Invoke();
        }

        // ======================================================
        // STATE HELPERS
        // ======================================================
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
        public void SetLoadingTasks(bool status)
        {
            IsLoadingTasks = status;
            Notify();
        }

        // ======================================================
        // LOGGING & CENTRAL PIPELINES
        // ======================================================
        public void LogActivity(string title, string action, string user = "Current User")
        {
            var newLog = new ActivityDto
            {
                Id = Guid.NewGuid(),
                Title = title,
                Action = action,
                CreatedBy = user,
                Timestamp = DateTime.Now
            };
            ActivityLogs.Insert(0, newLog);
            Notify();
        }

        public void LogGlobalAction(string category, string text, string icon, string uiTintClass)
        {
            SharedSystemLogs.Insert(0, new GlobalActivityItem
            {
                Category = category,
                Description = text,
                Icon = icon,
                ImpactClass = uiTintClass,
                Timestamp = DateTime.Now
            });
            Notify();
        }
        // ======================================================
        // DATA RELOAD METHODS
        // ======================================================
        public async Task LoadTasksAsync()
        {
            try
            {
                SetLoadingTasks(true);
                await Task.Delay(100);
                var now = DateTime.Now;
                Tasks = new List<TaskDto>
                {
                    new TaskDto { Id = Guid.NewGuid(), Title = "Inquiry about Product XYZ", CompanyName = "Tech Innovations Inc.", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Setup Sage X3 Connector", CompanyName = "Sage Internal", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Review Sales Forecast Q3", CompanyName = "Global Systems", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Verify Invoice #8821", CompanyName = "Brightwave", Priority = TaskPriority.Urgent, DueDate = DateTime.Today, Status = TaskStatus.Open },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Submit Expense Report", CompanyName = "Finance Dept", Priority = TaskPriority.High, DueDate = DateTime.Today, Status = TaskStatus.Open },
                    new TaskDto { Id = Guid.NewGuid(), Title = "Daily Team Sync", CompanyName = "Operational Team", Priority = TaskPriority.Medium, DueDate = DateTime.Today, Status = TaskStatus.Open }
                };

                foreach (var t in Tasks)
                {
                    if (t.CreatedDate == default) t.CreatedDate = now.AddDays(-1);
                    if (t.UpdatedDate == default) t.UpdatedDate = now;
                    if (string.IsNullOrEmpty(t.AssignedTo)) t.AssignedTo = "alex.turner@company.com";
                    if (string.IsNullOrEmpty(t.SourceEmailSubject)) t.SourceEmailSubject = "Auto-generated task";
                    if (string.IsNullOrEmpty(t.Description)) t.Description = "No additional details provided.";
                    if (string.IsNullOrEmpty(t.OutlookItemId)) t.OutlookItemId = Guid.NewGuid().ToString();
                }

                Notify();
            }
            finally
            {
                SetLoadingTasks(false);
            }
        }

        public void ReloadKnowledge(List<KnowledgeDto> articles)
        {
            KnowledgeBase = articles;
            Notify();
        }
        public void ReloadContacts(List<ContactCreateDto> contacts)
        {
            Contacts = contacts;
            Notify();
        }
        public void ReloadClients() { Notify(); }
        public void ReloadOrders() { Notify(); }
        public void ReloadTasks() { Notify(); }

        // ======================================================
        // 🚀 COMPILER FIX ALIAS: MAPS OPENORDERS TO QUOTES INFRASTRUCTURE [INDEX]
        // ======================================================
        public List<OpenOrderDto> OpenOrders
        {
            get => Quotes;
            set => Quotes = value;
        }

        // ======================================================
        // 🚀 COMPILER FIX ALIAS: MAPS AUDITLOGS TO ACTIVITYLOGS INFRASTRUCTURE [INDEX]
        // ======================================================
        public List<ActivityDto> AuditLogs
        {
            get => ActivityLogs;
            set => ActivityLogs = value;
        }

        // ======================================================
        // MEMORY CLEANUP
        // ======================================================
        public void Dispose()
        {
            OnChange = null;
        }
    }

    public class GlobalActivityItem
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ImpactClass { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}

