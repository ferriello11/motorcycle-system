using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;

namespace TesteTecnico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMotorcycleService _service;

        public MotorcyclesController(IMotorcycleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
        {
            try
            {
                var moto = await _service.CreateAsync(dto);
                return Ok(moto);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? plate)
        {
            var motos = await _service.GetAllAsync(plate);
            return Ok(motos);
        }

        [HttpPut("{id}/plate")]
        public async Task<IActionResult> UpdatePlate(int id, [FromBody] UpdateMotorcycleDto dto)
        {
            try
            {
                var moto = await _service.UpdatePlateAsync(id, dto.Plate);
                return Ok(moto);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
