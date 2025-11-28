using MESAmetrics.Dtos;
using MESAmetrics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MESAmetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelemetryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] TelemetryDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos invalidos"
                    });
                }

                var newTelemetry = new Telemetry
                {
                    CycleCount = request.CycleCount,
                    StopButton = request.StopButton,
                };

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Datos registrados correctamente"
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.ToString()
                });
            }
        }
    }
}