// CODE START: QuickActionUIService.cs

using System;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;

namespace OperationalWorkspaceUI.UIServices.Actions
{
    public class QuickActionUIService
    {
        private readonly IOrderService _orderService;
        private readonly ITaskService _taskService;
        private readonly IKnowledgeService _knowledgeService;
        private readonly IActivityService _activityService;

        public QuickActionUIService(
            IOrderService orderService,
            ITaskService taskService,
            IKnowledgeService knowledgeService,
            IActivityService activityService)
        {
            _orderService = orderService;
            _taskService = taskService;
            _knowledgeService = knowledgeService;
            _activityService = activityService;
        }

        // =========================
        // EMAIL → ORDER
        // =========================
        public async Task<OrderDto> CreateOrderFromEmailAsync(EmailInsightDto email)
        {
            var order = new OrderDto
            {
                ClientId = email.ClientId,
                OrderDate = DateTime.UtcNow,
                Description = $"Order from email: {email.Subject}"
            };

            var result = await _orderService.CreateOrderAsync(order);

            await LogActivity("Order Created from Email", email.Subject);

            return result;
        }

        // =========================
        // EMAIL → TASK
        // =========================
        public async Task<TaskDto> CreateTaskFromEmailAsync(EmailInsightDto email)
        {
            var task = new TaskDto
            {
                Title = $"Follow-up: {email.Subject}",
                Description = email.Body,
                AssignedToId = email.AssignedUserId,
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            var result = await _taskService.CreateTaskAsync(task);

            await LogActivity("Task Created from Email", email.Subject);

            return result;
        }

        // =========================
        // ATTACH EMAIL
        // =========================
        public async Task AttachEmailAsync(object model)
        {
            // TODO: Replace with strongly typed model later
            await _activityService.AttachEmailAsync(model);

            await LogActivity("Email Attached", "Email linked to record");
        }

        // =========================
        // CREATE QUOTE
        // =========================
        public async Task CreateQuoteAsync(object model)
        {
            // TODO: Replace with QuoteDTO when available
            await _orderService.CreateQuoteAsync(model);

            await LogActivity("Quote Created", "Quote created from UI");
        }

        // =========================
        // SEND KNOWLEDGE
        // =========================
        public async Task SendKnowledgeAsync(object model)
        {
            await _knowledgeService.SendKnowledgeAsync(model);

            await LogActivity("Knowledge Sent", "Knowledge article sent to client");
        }

        // =========================
        // SEARCH KNOWLEDGE
        // =========================
        public async Task<List<KnowledgeDto>> SearchKnowledgeAsync(string query)
        {
            return await _knowledgeService.SearchAsync(query);
        }

        // =========================
        // INTERNAL ACTIVITY LOGGER
        // =========================
        private async Task LogActivity(string action, string description)
        {
            await _activityService.LogAsync(new ActivityDto
            {
                Action = action,
                Description = description,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}

// CODE END