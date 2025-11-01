using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;

        public TasksController(ITaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll([FromQuery] string? priority = null)
        {
            var tasks = await _service.GetAllAsync(priority);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetById(ulong id)
        {
            var task = await _service.GetByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetByUser(ulong userId)
        {
            try
            {
                var tasks = await _service.GetByAssignedByAsync(userId);
                return Ok(tasks);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> Create(CreateTaskDto createTaskDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // Từ auth
                var created = await _service.CreateAsync(createTaskDto, currentUserId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> Update(ulong id, TaskDto taskDto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, taskDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> Delete(ulong id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (success == false)
                {
                    return NotFound("Không tìm thấy nhiệm vụ để hủy");
                }
                return Ok("Hủy thành công");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<TaskResponseDto>> Submit(ulong id, SubmitTaskDto submitDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUsername = GetCurrentUsername();

                var submitted = await _service.SubmitAsync(id, submitDto, currentUserId, currentUsername);
                return Ok(submitted);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<TaskResponseDto>> Confirm(ulong id)
        {
            try
            {
                var adminUserId = GetCurrentUserId();
                var adminUsername = GetCurrentUsername();

                var confirmed = await _service.ConfirmAsync(id, adminUserId, adminUsername);
                return Ok(confirmed);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/progress")]
        public async Task<ActionResult<TaskResponseDto>> UpdateProgress(ulong id, UpdateProgressDto progressDto)
        {
            try
            {
                var updated = await _service.UpdateTaskProgressAsync(id, progressDto);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private ulong GetCurrentUserId()
        {
            return ulong.Parse(User.FindFirst("userId")?.Value ?? "1");
        }

        private string GetCurrentUsername()
        {
            return User.FindFirst("username")?.Value ?? "MemeTest";
        }
    }
}
