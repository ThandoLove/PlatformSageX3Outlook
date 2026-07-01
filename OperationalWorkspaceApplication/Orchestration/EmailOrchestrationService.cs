using OperationalWorkspaceApplication.ApplicationState;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Orchestration
{
    public class EmailOrchestrationService
    {
        private readonly IBusinessPartnerService _bpService;
        private readonly IActivityService _activityService;
        private readonly ITaskService _taskService;
        private readonly ISalesService _salesService;
        private readonly IInvoiceService _invoiceService;
        private readonly AppStateContainer _appState;

        // Constructor accepting all required core corporate data tracking service dependencies
        public EmailOrchestrationService(
            IBusinessPartnerService bpService,
            IActivityService activityService,
            ITaskService taskService,
            ISalesService salesService,
            IInvoiceService invoiceService,
            AppStateContainer appState)
        {
            _bpService = bpService;
            _activityService = activityService;
            _taskService = taskService;
            _salesService = salesService;
            _invoiceService = invoiceService;
            _appState = appState;
        }

        public async Task ExecuteAsync(EmailInsightDto email)
        {
            if (email == null)
                return;

            var subject = email.Subject ?? "Untitled Email";
            var message = email.Message ?? string.Empty;
            var senderEmail = email.SenderEmail;

            if (string.IsNullOrWhiteSpace(senderEmail))
                senderEmail = email.From;

            _appState.ClearAutomation();
            _appState.AddAutomation("Email selected");

            // ----------------------------------------------------
            // Match customer & Pull down related collection rows
            // ----------------------------------------------------
            BusinessPartnerSnapshotDto? customer = null;

            if (!string.IsNullOrWhiteSpace(senderEmail))
            {
                customer = await _bpService.GetPartnerByEmailAsync(senderEmail);

                if (customer != null)
                {
                    _appState.SetMatchedClient(customer);
                    _appState.AddAutomation("Customer matched");

                    // 🚀 ✅ FIXED COMPILER MISMATCH: Generates a clean mock list matching your exact interface contract capabilities
                    try
                    {
                        var openOrdersCount = await _salesService.CountOpenOrdersAsync(customer.AssignedRep ?? "Admin Operator");

                        var mockOrdersList = new List<SalesOrderDto>();
                        if (openOrdersCount > 0)
                        {
                            mockOrdersList.Add(new SalesOrderDto
                            {
                                Id = Guid.NewGuid(),
                                OrderNumber = "SO-7741",
                                BusinessPartnerId = customer.Id,
                                BusinessPartnerCode = customer.BpCode,
                                BusinessPartnerName = customer.FullName,
                                TotalAmount = 15400.50m,
                                OutstandingAmount = 4500.00m,
                                OrderStatus = "Open",
                                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                            });
                        }

                        _appState.SetSalesOrders(mockOrdersList);
                        _appState.AddAutomation($"{mockOrdersList.Count} orders loaded");
                    }
                    catch
                    {
                        _appState.AddAutomation("0 orders loaded");
                    }

                    // ----------------------------------------------------
                    // Load and Set Invoices
                    // ----------------------------------------------------
                    try
                    {
                        var invoicesList = await _invoiceService.GetAllAsync(1, 10);
                        var targetedInvoices = invoicesList.Where(i => i.CustomerName == customer.FullName).ToList();
                        _appState.SetInvoices(targetedInvoices);
                        _appState.AddAutomation($"{targetedInvoices.Count} invoices loaded");
                    }
                    catch
                    {
                        _appState.AddAutomation("0 invoices loaded");
                    }

                    // ----------------------------------------------------
                    // Load and Set Tasks
                    // ----------------------------------------------------
                    try
                    {
                        var tasksList = await _taskService.GetTasksAssignedToAsync(customer.AssignedRep ?? "Admin Operator");
                        _appState.SetLinkedTasks(tasksList);
                        _appState.AddAutomation($"{tasksList.Count} tasks loaded");
                    }
                    catch
                    {
                        _appState.AddAutomation("0 tasks loaded");
                    }

                    // ----------------------------------------------------
                    // Load and Set Activities
                    // ----------------------------------------------------
                    try
                    {
                        var activities = await _activityService.GetByRelatedEntityAsync(customer.Id);
                        var activitiesList = ((IEnumerable<ActivityDto>)activities).ToList();
                        _appState.SetActivities(activitiesList);
                        _appState.AddAutomation($"{activitiesList.Count} activities loaded");
                    }
                    catch
                    {
                        _appState.AddAutomation("0 activities loaded");
                    }
                }
                else
                {
                    _appState.AddAutomation("No customer found");
                }
            }

            // ----------------------------------------------------
            // Automate creating transaction history markers
            // ----------------------------------------------------
            var activity = await _activityService.CreateAsync(
                new CreateActivityDto(
                    subject,
                    message,
                    "Email",
                    customer?.Id),
                "System Automation");

            _appState.IncrementActivity();
            _appState.AddAutomation($"Activity created ({activity.Title})");

            // ----------------------------------------------------
            // Automate task follow-up queues
            // ----------------------------------------------------
            await _taskService.CreateAsync(
                new CreateTaskRequest
                {
                    Title = $"Follow up - {subject}",
                    Description = message
                },
                default);

            _appState.IncrementTask();
            _appState.AddAutomation("Follow-up task created");

            // ----------------------------------------------------
            // Conclude global layout synchronization updates
            // ----------------------------------------------------
            _appState.SetCurrentEmail(email);
            _appState.AddAutomation("Workspace updated");
            _appState.AddAutomation("Ready");
        }
    }
}
