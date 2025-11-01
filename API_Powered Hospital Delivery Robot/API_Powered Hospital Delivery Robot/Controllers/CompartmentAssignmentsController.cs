using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompartmentAssignmentsController : ControllerBase
    {
        private readonly ICompartmentAssignmentService _service;

        public CompartmentAssignmentsController(ICompartmentAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompartmentAssignmentResponseDto>>> GetAll([FromQuery] ulong? taskId = null, [FromQuery] string? status = null)
        {
            var assignments = await _service.GetAllAsync(taskId, status);
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompartmentAssignmentResponseDto>> GetById(ulong id)
        {
            var assignment = await _service.GetByIdAsync(id);
            if (assignment == null) return NotFound();
            return Ok(assignment);
        }

        [HttpPost]
        public async Task<ActionResult<CompartmentAssignmentResponseDto>> Create(CompartmentAssignmentDto assignmentDto)
        {
            try
            {
                var created = await _service.CreateAsync(assignmentDto);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<CompartmentAssignmentResponseDto>> Update(ulong id, CompartmentAssignmentDto assignmentDto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, assignmentDto);
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

        [HttpPatch("{id}/load")]
        public async Task<ActionResult<CompartmentAssignmentResponseDto>> Load(ulong id, LoadCompartmentDto loadDto)
        {
            try
            {
                var loaded = await _service.LoadAsync(id, loadDto);
                if (loaded == null) return NotFound();
                return Ok(loaded);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Bulk load cho task
        [HttpPost("tasks/{taskId}/load-compartments")]
        public async Task<ActionResult<IEnumerable<CompartmentAssignmentResponseDto>>> BulkLoad(ulong taskId, List<LoadCompartmentDto> loadDtos)
        {
            try
            {
                var loaded = await _service.BulkLoadForTaskAsync(taskId, loadDtos);
                return Ok(loaded);
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
    }
}
