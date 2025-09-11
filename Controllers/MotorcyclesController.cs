using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;

namespace TesteTecnico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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

        [HttpPut("plate")]
        public async Task<IActionResult> UpdatePlate([FromBody] UpdateMotorcycleDto dto)
        {
            try
            {
                var moto = await _service.UpdatePlateAsync(dto.OldPlate, dto.NewPlate);
                return Ok(moto);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{plate}")]
        public async Task<IActionResult> Delete(string plate)
        {
            try
            {
                await _service.DeleteAsync(plate);
                return Ok(new { message = $"Moto com placa {plate} deletada com sucesso." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
