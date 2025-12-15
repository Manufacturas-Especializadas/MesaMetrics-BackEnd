using MESAmetrics.Dtos;
using MESAmetrics.Hubs;
using MESAmetrics.Models;
using MESAmetrics.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MESAmetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MachineHub> _hubContext;
        private readonly IMetricsService _metricsService;

        public TelemetryController(
                AppDbContext context,
                IHubContext<MachineHub> hubContext,
                IMetricsService metricsService)
        {
            _context = context;
            _hubContext = hubContext;
            _metricsService = metricsService;
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

        [HttpGet]
        [Route("CurrentMetrics/{realTimeId}")]
        public async Task<IActionResult> GetCurrentMetrics(int realTimeId)
        {
            var metrics = await _metricsService.CalculateMetricsAsync(realTimeId);

            if (metrics == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se encontrarón datos para esta sesión"
                });
            }

            return Ok(metrics);
        }

        [HttpGet]
        [Route("GetActiveSessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            var today = DateTime.Today;

            var activeIds = await _context.RealTime              
                .Where(rt => rt.EndTime == null || rt.CreatedAt!.Value.Date == today)
                .Select(rt => rt.Id)
                .ToListAsync();

            return Ok(activeIds);
        }

        [HttpGet]
        [Route("DashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var today = DateTime.Today;
            var activeIds = await _context.RealTime
                    .Where(rt => rt.EndTime == null || rt.CreatedAt!.Value.Date == today)
                    .Select(rt => rt.Id)
                    .ToListAsync();

            int produciendo = 0;
            int detenido = 0;
            int alerta = 0;
            int sinDatos = 0;

            foreach(var id in activeIds)
            {
                var metrics = await _metricsService.CalculateMetricsAsync(id);

                if(metrics != null)
                {
                    switch (metrics.Status?.ToLower())
                    {
                        case "produccion":
                            produciendo++;
                            break;

                        case "detenido":
                            detenido++; 
                            break;

                        case "offline":
                            sinDatos++; 
                            break;

                        default:
                            alerta++;
                            break;
                    }
                }
                else
                {
                    sinDatos++;
                }
            }

            return Ok(new
            {
                Produciendo = produciendo,
                Detenido = detenido,
                Alerta = alerta,
                SinDatos = sinDatos,
                SinTurno = 0,
                Total = activeIds.Count
            });
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
                    Active = request.Active,
                    RealTimeId = request.RealTimeId,
                    CreatedAt = DateTime.Now
                };

                _context.Telemetry.Add(newTelemetry);
                await _context.SaveChangesAsync();

                if (request.RealTimeId.HasValue)
                {
                    var metrics = await _metricsService.CalculateMetricsAsync(request.RealTimeId.Value);

                    if(metrics != null)
                    {
                        await _hubContext.Clients.Groups(request.RealTimeId.Value.ToString())
                                .SendAsync("ReceiveMachineMetrics", metrics);

                        await _hubContext.Clients.All.SendAsync("RefreshDashboard");
                    }
                }

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