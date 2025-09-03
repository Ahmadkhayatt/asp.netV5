using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // 🔑 Only Admins can access this controller
    public class AdminController : ControllerBase
    {
        [HttpGet("all-tasks")]
        public IActionResult GetAllTasks()
        {
            return Ok("This would return ALL tasks (Admin only).");
        }

        [HttpPost("create-user")]
        public IActionResult CreateUser()
        {
            return Ok("This would allow Admin to create a new user.");
        }
    }
}
