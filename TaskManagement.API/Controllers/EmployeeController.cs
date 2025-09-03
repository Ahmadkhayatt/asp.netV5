using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")]  // 🔑 Only Employees can access
    public class EmployeeController : ControllerBase
    {
        [HttpGet("my-tasks")]
        public IActionResult GetMyTasks()
        {
            // 🔎 Extract user ID from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok($"This would return tasks only for user {userId}.");
        }

        [HttpPut("update-task/{taskId}")]
        public IActionResult UpdateTask(int taskId)
        {
            // Only employee’s own tasks will be updatable
            return Ok($"This would update task {taskId} (Employee only).");
        }
    }
}
