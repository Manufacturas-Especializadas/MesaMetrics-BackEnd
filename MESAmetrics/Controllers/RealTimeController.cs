using MESAmetrics.Dtos;
using MESAmetrics.Models;
using MESAmetrics.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MESAmetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RealTimeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("RegisterRealTime")]
        public async Task<IActionResult> RegisterRealTime([FromBody] RealTimeDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "El título de la maquina es requerido"
                    });
                }

                if (request.ShiftId <= 0)
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "El turno es requerido"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.StartTime) || string.IsNullOrWhiteSpace(request.EndTime))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Las horas de inicio y fin son requeridas"
                    });
                }

                if (request.TagsId == null || !request.TagsId.Any())
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Se requiere al menos una etiqueta"
                    });
                }

                if (!TimeOnly.TryParse(request.StartTime, out TimeOnly startTimeParsed))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Formato de hora de inicio inválido. Use formato HH:mm"
                    });
                }

                if (!TimeOnly.TryParse(request.EndTime, out TimeOnly endTimeParsed))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Formato de hora de fin inválido. Use formato HH:mm"
                    });
                }

                var newRegister = new RealTime
                {
                    Title = request.Title.Trim(),
                    ShiftId = request.ShiftId,
                    StartTime = startTimeParsed,
                    EndTime = endTimeParsed,
                    LineId = request.LineId,
                };

                foreach (var tagId in request.TagsId)
                {
                    newRegister.RealTimeTags.Add(new RealTimeTags
                    {
                        TagId = tagId,
                    });
                }

                await _context.RealTime.AddAsync(newRegister);
                await _context.SaveChangesAsync();

                return Ok(new RealTimeResponse
                {
                    Success = true,
                    Message = "Registro guardado exitosamente",
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new RealTimeResponse
                {
                    Success = false,
                    Message = $"Erorr al guardar el registro: {ex.Message}"
                });
            }
        }
    }
}