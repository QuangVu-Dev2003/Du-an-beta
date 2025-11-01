using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Powered_Hospital_Delivery_Robot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _service;

        public MedicinesController(IMedicineService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineResponseDto>>> GetAll([FromQuery] ulong? categoryId = null, [FromQuery] string? status = null)
        {
            var medicines = await _service.GetAllAsync(categoryId, status == null ? null : (MedicineStatus?)Enum.Parse(typeof(MedicineStatus), status));
            return Ok(medicines);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineResponseDto>> GetById(ulong id)
        {
            var medicine = await _service.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return Ok(medicine);
        }

        [HttpPost]
        public async Task<ActionResult<MedicineResponseDto>> Create(MedicineDto medicineDto)
        {
            try
            {
                var created = await _service.CreateAsync(medicineDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicineResponseDto>> Update(ulong id, MedicineDto medicineDto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, medicineDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("scan-expired")]
        public async Task<ActionResult<ScanExpiredResponseDto>> ScanExpired([FromBody] ScanExpiredDto scanDto)
        {
            try
            {
                var response = await _service.ScanExpiredAsync(scanDto.FlagOnly);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove-expired")]
        public async Task<ActionResult<int>> RemoveExpired()
        {
            try
            {
                var count = await _service.RemoveExpiredAsync();
                return Ok(count);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
