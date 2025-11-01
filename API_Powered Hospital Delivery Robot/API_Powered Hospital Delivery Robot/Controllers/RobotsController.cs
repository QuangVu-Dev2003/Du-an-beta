using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RobotsController : ControllerBase
    {
        private readonly IRobotService _service;

        public RobotsController(IRobotService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RobotResponseDto>>> GetAll([FromQuery] string? status = null)
        {
            var robots = await _service.GetAllAsync(status);
            return Ok(robots);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RobotResponseDto>> GetById(ulong id)
        {
            var robot = await _service.GetByIdAsync(id);
            if (robot == null) return NotFound();
            // Nếu có MapId, load map và tính position relative (thêm vào DTO nếu cần)
            if (robot.MapId.HasValue)
            {
                //var map = await _mapService.GetByIdAsync(robot.MapId.Value);
                // var (x, y) = _service.CalculatePositionOnMap(robot, map);
                // robot.PositionOnMap = new { X = x, Y = y }; // Extend DTO nếu cần
            }
            return Ok(robot); // Include Compartments & Tasks
        }

        [HttpPost]
        public async Task<ActionResult<RobotResponseDto>> Create(RobotDto robotDto)
        {
            try
            {
                var created = await _service.CreateAsync(robotDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<RobotResponseDto>> UpdateStatus(ulong id, UpdateStatusDto statusDto)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(id, statusDto);
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

        [HttpPut("{robotId}/assign-map/{mapId}")]
        public async Task<ActionResult<AssignMapResponseDto>> AssignMap(ulong robotId, ulong mapId)
        {
            try
            {
                var response = await _service.AssignMapAsync(robotId, mapId);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/position")]
        public async Task<ActionResult<RobotResponseDto>> UpdatePosition(ulong id, UpdatePositionDto positionDto)
        {
            try
            {
                var updated = await _service.UpdatePositionAsync(id, positionDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
