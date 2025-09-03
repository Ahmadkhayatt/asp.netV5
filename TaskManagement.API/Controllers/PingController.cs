using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        // فحص سريع بدون توكن
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get() => Ok(new { ok = true, service = "TaskManagement.API" });
    }
}
