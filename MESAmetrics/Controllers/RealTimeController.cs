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
                if (request == null)
                {
                    return BadRequest(new RealTimeResponse
                    {
                        Success = false,
                        Message = "Hay campo incompletos"
                    });
                }

                var newRegister = new RealTime
                {
                    Title = request.Title,
                    ShiftId = request.ShiftId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    TagsId = request.TagsId,
                };

                await _context.RealTime.AddAsync(newRegister);
                await _context.SaveChangesAsync();

                return Ok(new RealTimeResponse
                {
                    Success = true,
                    Message = "Registro guardado"
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new RealTimeResponse
                {
                    Success = false,
                    Message = $"No se pudo guardar el registro: {ex.Message}"
                });
            }
        }
    }
}