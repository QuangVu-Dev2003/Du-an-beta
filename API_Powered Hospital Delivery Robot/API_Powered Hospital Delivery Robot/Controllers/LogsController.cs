using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _service;

        public LogsController(ILogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogResponseDto>>> GetAll([FromQuery] ulong? robotId = null, [FromQuery] ulong? taskId = null, [FromQuery] string? logType = null)
        {
            var logs = await _service.GetAllAsync(robotId, taskId, logType);
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LogResponseDto>> GetById(ulong id)
        {
            var log = await _service.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpPost]
        public async Task<ActionResult<LogResponseDto>> Create(LogDto logDto)
        {
            try
            {
                var created = await _service.CreateAsync(logDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}