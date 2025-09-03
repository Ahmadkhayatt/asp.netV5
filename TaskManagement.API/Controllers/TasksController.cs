// TaskManagement.API/Controllers/TasksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using CoreTaskStatus = TaskManagement.Core.Entities.TaskStatus;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // DTO محلي للاستجابات لتفادي حلقات المراجع
        private sealed record TaskResponseDto(
            int Id,
            string Title,
            string Description,
            string Status,
            int AssignedToUserId,
            DateTime CreatedAt
        );

        // Admin: Get all tasks (مسقطة إلى DTO)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Tasks
                .AsNoTracking()
                .Select(t => new TaskResponseDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.AssignedToUserId,
                    t.CreatedAt
                ))
                .ToListAsync();

            return Ok(data);
        }

        // Employee: Get my tasks
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            var data = await _context.Tasks
                .AsNoTracking()
                .Where(t => t.AssignedToUserId == userId)
                .Select(t => new TaskResponseDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.AssignedToUserId,
                    t.CreatedAt
                ))
                .ToListAsync();

            return Ok(data);
        }

        // Admin: Create task
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                AssignedToUserId = dto.AssignedToUserId,
                Status = CoreTaskStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var result = new TaskResponseDto(
                task.Id, task.Title, task.Description, task.Status.ToString(),
                task.AssignedToUserId, task.CreatedAt
            );

            return Ok(result);
        }

        // Admin: Update task
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.AssignedToUserId = dto.AssignedToUserId;
            task.Status = Enum.Parse<CoreTaskStatus>(dto.Status, true);

            await _context.SaveChangesAsync();

            var result = new TaskResponseDto(
                task.Id, task.Title, task.Description, task.Status.ToString(),
                task.AssignedToUserId, task.CreatedAt
            );

            return Ok(result);
        }

        // Employee: Update only my task status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskStatusDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedToUserId == userId);

            if (task == null) return NotFound("Task not found or not yours.");

            task.Status = Enum.Parse<CoreTaskStatus>(dto.Status, true);
            await _context.SaveChangesAsync();

            var result = new TaskResponseDto(
                task.Id, task.Title, task.Description, task.Status.ToString(),
                task.AssignedToUserId, task.CreatedAt
            );

            return Ok(result);
        }

        // Admin: Delete task
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok("Task deleted.");
        }
    }
}
