  
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IServices;
using OperationalWorkspaceApplication.Services;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers
    {
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserContextService _userContextService;

        public DashboardController(IDashboardService dashboardService, IUserContextService userContextService)
        {
            _dashboardService = dashboardService;
            _userContextService = userContextService;
        }

        [HttpGet("employee")]
        public async Task<ActionResult<EmployeeDashboardDTO>> GetEmployeeDashboard()
        {
            // 1. Get the full user object since GetCurrentUserId doesn't exist
            var user = await _userContextService.GetCurrentUserAsync();

            if (user == null || string.IsNullOrEmpty(user.Id))
                return Unauthorized();

            var dashboard = await _dashboardService.GetEmployeeDashboardAsync(user.Id);
            return Ok(dashboard);
        }

        [HttpGet("admin")]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
        {
            var user = await _userContextService.GetCurrentUserAsync();

            // 2. Check the Role string instead of a non-existent IsAdmin property
            if (user == null || user.Role != "Admin")
                return Forbid();

            var dashboard = await _dashboardService.GetAdminDashboardAsync();
            return Ok(dashboard);
        }

    }  // CODE END
 }
