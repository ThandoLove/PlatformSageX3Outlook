using Microsoft.AspNetCore.Mvc;
using OperationalWorkspaceAPI.Models;
using OperationalWorkspaceAPI.Services;

namespace OperationalWorkspaceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] TestEmailDto dto)
        {
            TestEmailStore.Latest = dto;
            return Ok(new { message = "Stored" });
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            if (TestEmailStore.Latest == null)
                return NoContent();

            return Ok(TestEmailStore.Latest);
        }
    }
}
