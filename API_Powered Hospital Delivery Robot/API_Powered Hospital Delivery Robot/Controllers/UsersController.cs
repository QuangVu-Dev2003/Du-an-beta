using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll([FromQuery] bool? isActive = null)
        {
            var users = await _service.GetAllAsync(isActive);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(ulong id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user); // Include Tasks và ActiveSessions
        }

        // Real-time status (polling endpoint)
        [HttpGet("{id}/status")]
        public async Task<ActionResult<UserStatusDto>> GetStatus(ulong id)
        {
            try
            {
                var status = await _service.GetUserStatusAsync(id);
                return Ok(status); // IsOnline, ActiveSessions, LastActivity
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create(UserDto userDto)
        {
            try
            {
                var created = await _service.CreateAsync(userDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> Update(ulong id, UserDto userDto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, userDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(ulong id)
        {
            try
            {
                var success = await _service.ToggleActiveAsync(id, true);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(ulong id)
        {
            try
            {
                var success = await _service.ToggleActiveAsync(id, false);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
