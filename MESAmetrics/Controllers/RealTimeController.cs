using MESAmetrics.Dtos;
using MESAmetrics.Models;
using MESAmetrics.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        [Route("GetRealTimeById/{id}")]
        public async Task<IActionResult> GetRealTimeById(int id)
        {
            var realTime = await _context.RealTime
                .Select(r => new
                {
                    Id = r.Id,
                    Title = r.Title,
                    Shift = r.Shift.ShiftName,
                    Line = r.Line.LinesName,
                    MachineId = r.Machine.Machine,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    TagsId = r.RealTimeTags.Select(r => r.Id).ToList()
                })
                .FirstOrDefaultAsync(r => r.Id == id);

            return Ok(realTime);
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

        [HttpPut]
        [Route("UpdateRealTime/{id}")]
        public async Task<IActionResult> UpdateRealTime([FromBody] RealTimeDto request, int id)
        {
            try
            {
                if(request == null || string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "El titulo de la maquina es requerido"
                    });
                }

                if(!TimeOnly.TryParse(request.StartTime, out TimeOnly startTimeParsed) || 
                    !TimeOnly.TryParse(request.EndTime, out TimeOnly endTimeParsed))
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Formato de horas inválido"
                    });
                }

                var machine = await _context.RealTime
                        .Include(x => x.RealTimeTags)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if(machine == null)
                {
                    return NotFound(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Id no encontrado"
                    });
                }

                machine.Title = request.Title.Trim();
                machine.ShiftId = request.ShiftId;
                machine.StartTime = startTimeParsed;
                machine.EndTime = endTimeParsed;
                machine.LineId = request.LineId;
                machine.UpdatedAt = DateTime.UtcNow;

                if(request.TagsId != null)
                {
                    var tagsToDelete = machine.RealTimeTags
                            .Where(t => !request.TagsId.Contains(t.TagId))
                            .ToList();

                    foreach(var tag in tagsToDelete)
                    {
                        machine.RealTimeTags.Remove(tag);
                    }

                    var existingTagIds = machine.RealTimeTags.Select(t => t.TagId).ToList();
                    var tagsToAdd = request.TagsId
                            .Where(newId => !existingTagIds.Contains(newId))
                            .ToList();

                    foreach(var tagId in tagsToAdd)
                    {
                        machine.RealTimeTags.Add(new RealTimeTags
                        {
                            TagId = tagId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new RealTimeResponse
                {
                    Success = true,
                    Message = "Registro actualizado"
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new RealTimeResponse
                {
                    Success = false,
                    Message = $"Error al actualizar: {ex.Message}"
                });
            }
        }
    }
}