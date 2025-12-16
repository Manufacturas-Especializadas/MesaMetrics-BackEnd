using MESAmetrics.Dtos;
using MESAmetrics.Models;
using Microsoft.EntityFrameworkCore;

namespace MESAmetrics.Services
{
    public interface IMetricsService
    {
        Task<MachineMetricsDto> CalculateMetricsAsync(int realTimeId);
    }

    public class MetricsService : IMetricsService
    {
        private readonly AppDbContext _context;

        public MetricsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MachineMetricsDto> CalculateMetricsAsync(int realTimeId)
        {
            var session = await _context.RealTime
                .Include(rt => rt.Shift)
                .Include(rt => rt.Telemetry)
                .FirstOrDefaultAsync(rt => rt.Id == realTimeId);

            if (session == null) return null!;

            var logs = session.Telemetry.OrderBy(t => t.CreatedAt).ToList();

            double productionSeconds = 0;
            double stopSeconds = 0;
            int stopCount = 0;

            for (int i = 0; i < logs.Count - 1; i++)
            {
                var current = logs[i];
                var next = logs[i + 1];
                var duration = (next.CreatedAt!.Value - current.CreatedAt!.Value).TotalSeconds;

                if (current.CycleCount == 1)
                {
                    productionSeconds += duration;
                }
                else
                {
                    stopSeconds += duration;

                    if (i > 0 && logs[i - 1].CycleCount == 1) stopCount++;
                }
            }

            string currentStatus = "offline";
            string statusDurationStr = "0m";
            double currentSegmentDuration = 0;

            if (logs.Any())
            {
                var lastLog = logs.Last();
                var now = DateTime.Now;
                var timeSinceLastSignal = (now - lastLog.CreatedAt!.Value).TotalSeconds;
                
                bool isProducing = lastLog.CycleCount == 1;
                currentStatus = isProducing ? "produccion" : "detenido";

                if (isProducing) productionSeconds += timeSinceLastSignal;
                else stopSeconds += timeSinceLastSignal;

                var lastStateChangeTime = lastLog.CreatedAt!.Value;

                for (int i = logs.Count - 2; i >= 0; i--)
                {
                    bool prevWasProducing = logs[i].CycleCount == 1;
                    if (prevWasProducing != isProducing)
                    {
                        break;
                    }
                    lastStateChangeTime = logs[i].CreatedAt!.Value;
                }

                var durationSpan = now - lastStateChangeTime;
                statusDurationStr = FormatTime(durationSpan);
            }

            double totalTime = productionSeconds + stopSeconds;
            double availability = totalTime > 0 ? (productionSeconds / totalTime) * 100 : 0;

            return new MachineMetricsDto
            {
                MachineName = session.Title,
                Shift = session.Shift?.ShiftName ?? "N/A",
                Status = currentStatus,
                StatusDuration = statusDurationStr,
                Availability = $"{availability:F1}%",
                ProductionTime = FormatTime(TimeSpan.FromSeconds(productionSeconds)),
                StopTime = FormatTime(TimeSpan.FromSeconds(stopSeconds)),
                Stops = stopCount.ToString()
            };
        }

        private string FormatTime(TimeSpan ts)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        }
    }
}