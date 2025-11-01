using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly IMapService _service;

        public MapsController(IMapService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MapResponseDto>>> GetAll()
        {
            var maps = await _service.GetAllAsync();
            return Ok(maps);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MapResponseDto>> GetById(ulong id)
        {
            var map = await _service.GetByIdAsync(id);
            if (map == null) return NotFound();
            return Ok(map); // Include Robots
        }

        // Serve image file
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetImage(ulong id)
        {
            var map = await _service.GetByIdAsync(id);
            if (map == null || map.ImageData == null) return NotFound();

            var imageName = map.ImageName ?? "map.png";
            return File(map.ImageData, "image/png", imageName); 
        }

        [HttpPost]
        public async Task<ActionResult<MapResponseDto>> Create([FromForm] MapDto mapDto, IFormFile? imageFile)
        {
            try
            {
                // Validate file nếu có
                if (imageFile != null && imageFile.Length > 10 * 1024 * 1024) // 10MB limit
                    return BadRequest("Image file too large (max 10MB)");

                var created = await _service.CreateAsync(mapDto, imageFile);
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
        public async Task<ActionResult<MapResponseDto>> Update(ulong id, [FromForm] MapDto mapDto, IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 10 * 1024 * 1024)
                    return BadRequest("Image file too large (max 10MB)");

                var updated = await _service.UpdateAsync(id, mapDto, imageFile);
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
    }
}
