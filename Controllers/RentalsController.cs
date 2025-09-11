using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;

namespace TesteTecnico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateRentalDto dto)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                    return Unauthorized(new { message = "Usuário não autenticado." });

                var userIdClaim = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

                var userId = Guid.Parse(userIdClaim.Value);
                var rental = await _rentalService.CreateRentalAsync(userId, dto);
                return Ok(rental);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnRental([FromBody] ReturnRentalDto dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Usuário não autenticado." });

                var userId = Guid.Parse(userIdClaim.Value);
                var rental = await _rentalService.ReturnRentalAsync(userId, dto);

                return Ok(new
                {
                    Plate = dto.Plate,
                    TotalCost = rental.TotalCost,
                    StartDate = rental.StartDate,
                    ExpectedEndDate = rental.ExpectedEndDate,
                    EndDate = rental.EndDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
