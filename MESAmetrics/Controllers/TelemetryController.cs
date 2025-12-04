using MESAmetrics.Dtos;
using MESAmetrics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        [Route("GetTelemetry")]
        public async Task<IActionResult> GetTelemetry()
        {
            var telemetry = await _context.Telemetry
                                    .AsNoTracking()
                                    .ToListAsync();

            if(telemetry == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Sin datos"
                });
            }

            return Ok(telemetry);
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
                    MessageId = request.MessageId,
                };

                _context.Telemetry.Add(newTelemetry);
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

        [HttpPost]
        [Route("MachineRegistration")]
        public async Task<IActionResult> MachineRegistration([FromBody] MachinesIdsDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No puede enviar datos nulos"
                    });
                }

                var newMachine = new MachinesIds
                {
                    Machine = request.Machine
                };

                _context.MachinesIds.Add(newMachine);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Registro creado"
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al registrar: ${ex.Message}"
                });
            }
        }

        [HttpDelete]
        [Route("MachineDelete/{id}")]
        public async Task<IActionResult> MachineDelete(int id)
        {
            var machine = await _context.MachinesIds.FindAsync(id);

            if(machine == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Id no encontrado"
                });
            }

            _context.MachinesIds.Remove(machine);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Id eliminado"
            });
        }
    }
}