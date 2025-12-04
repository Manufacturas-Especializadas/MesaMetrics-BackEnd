using MESAmetrics.Models;
using MESAmetrics.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESAmetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralListController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeneralListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetShifts")]
        public async Task<IActionResult> GetShifts()
        {
            try
            {
                var shifts = await _context.Shifts
                                    .AsNoTracking()
                                    .ToListAsync();

                if (shifts == null || shifts.Count == 0)
                {
                    return Ok(new GeneralListsResponse<Shifts[]>
                    {
                        Success = false,
                        Message = "Sin datos",
                        Data = Array.Empty<Shifts>()
                    });
                }

                return Ok(new GeneralListsResponse<Shifts[]>
                {
                    Success = true,
                    Message = "Datos obtenidos correctamente",
                    Data = shifts.ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralListsResponse<Shifts[]>
                {
                    Success = false,
                    Message = $"Error interno: {ex.Message}",
                    Data = Array.Empty<Shifts>()
                });
            }
        }

        [HttpGet]
        [Route("GetTags")]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                var tags = await _context.Tags
                                    .AsNoTracking()
                                    .ToListAsync();

                if (tags == null || tags.Count == 0)
                {
                    return Ok(new GeneralListsResponse<Tags[]>
                    {
                        Success = false,
                        Message = "Sin datos",
                        Data = Array.Empty<Tags>()
                    });
                }

                return Ok(new GeneralListsResponse<Tags[]>
                {
                    Success = true,
                    Message = "Datos obtenidos correctamente",
                    Data = tags.ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralListsResponse<Tags[]>
                {
                    Success = false,
                    Message = $"Error interno: {ex.Message}",
                    Data = Array.Empty<Tags>()
                });
            }
        }

        [HttpGet]
        [Route("GetMachineIds")]
        public async Task<IActionResult> GetMachineIds()
        {
            try
            {
                var machine = await _context.MachinesIds
                                .AsNoTracking()
                                .ToListAsync();

                if(machine == null || machine.Count == 0)
                {
                    return BadRequest(new GeneralListsResponse<MachinesIds[]>
                    {
                        Success = false,
                        Message = "Sin datos"
                    });
                }

                return Ok(new GeneralListsResponse<MachinesIds[]>
                {
                    Success = true,
                    Message = "Datos obtenidos correctamente",
                    Data = machine.ToArray()
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new GeneralListsResponse<MachinesIds[]>
                {
                    Success = false,
                    Message = $"Error interno: {ex.Message}",
                    Data = Array.Empty<MachinesIds>()
                });
            }
        }
    }
}