using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Requests;

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

        public async Task CreateClientFromEmailAsync() => await Task.CompletedTask;
        public async Task OpenClientSearchAsync() => await Task.CompletedTask;

        public async Task CreateTaskFromEmailAsync(EmailInsightDto email)
        {
            // FIX: Using the positional constructor required by your CreateTaskRequest
            var request = new CreateTaskRequest(
                $"Follow-up: {email.Subject}",
                email.Message,
                email.AssignedUserId,
                DateTime.UtcNow.AddDays(3)
            );

            await _taskService.CreateAsync(request, CancellationToken.None);
            await LogActivity("Task Created from Email", email.Subject);
        }

        public async Task AttachEmailAsync(object model)
        {
            await _activityService.AttachEmailAsync(model);
            await LogActivity("Email Attached", "Email linked");
        }

        public async Task SendKnowledgeAsync(object model)
        {
            await _knowledgeService.SendKnowledgeAsync(model);
            await LogActivity("Knowledge Sent", "Sent to client");
        }

        public async Task<List<KnowledgeDto>> SearchKnowledgeAsync(string query)
        {
            var results = await _knowledgeService.SearchAsync(query);
            return results.ToList();
        }

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
