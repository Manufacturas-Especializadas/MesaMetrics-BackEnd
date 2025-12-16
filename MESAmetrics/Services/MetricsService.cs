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

            var startOfDay = DateTime.Today;
            var logs = session.Telemetry
                .Where(t => t.CreatedAt >= startOfDay)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            double productionSeconds = 0;
            double stopSeconds = 0;
            int stopCount = 0;

            bool wasProducing = false;

            for (int i = 0; i < logs.Count - 1; i++)
            {
                var current = logs[i];
                var next = logs[i + 1];

                var duration = (next.CreatedAt!.Value - current.CreatedAt!.Value).TotalSeconds;

                if (duration > 60)
                {
                    stopSeconds += duration;
                    if (wasProducing) { stopCount++; wasProducing = false; }
                }
                else
                {
                    bool isIntervalProducing = next.CycleCount > current.CycleCount;

                    if (isIntervalProducing)
                    {
                        productionSeconds += duration;
                        wasProducing = true;
                    }
                    else
                    {
                        stopSeconds += duration;
                        if (wasProducing) { stopCount++; wasProducing = false; }
                    }
                }
            }

            string currentStatus = "offline";
            string statusDurationStr = "0m";

            if (logs.Any())
            {
                var lastLog = logs.Last();
                var now = DateTime.Now;

                var timeSinceLastSignal = (now - lastLog.CreatedAt!.Value).TotalSeconds;

                if (timeSinceLastSignal > 60)
                {
                    currentStatus = "detenido";

                    stopSeconds += timeSinceLastSignal;

                    statusDurationStr = FormatTime(TimeSpan.FromSeconds(timeSinceLastSignal));
                }
                else
                {
                    bool isCurrentlyProducing = false;

                    if (logs.Count >= 2)
                    {
                        var penultimate = logs[logs.Count - 2];
                        isCurrentlyProducing = lastLog.CycleCount > penultimate.CycleCount;
                    }

                    currentStatus = isCurrentlyProducing ? "produccion" : "detenido";

                    if (isCurrentlyProducing)
                    {
                        productionSeconds += timeSinceLastSignal;
                        statusDurationStr = "Produciendo..."; 
                    }
                    else
                    {
                        stopSeconds += timeSinceLastSignal;

                        var lastStateChangeTime = lastLog.CreatedAt!.Value;
                        for (int i = logs.Count - 2; i >= 0; i--)
                        {
                            if ((logs[i + 1].CycleCount > logs[i].CycleCount) != isCurrentlyProducing)
                            {
                                lastStateChangeTime = logs[i + 1].CreatedAt!.Value;
                                break;
                            }
                        }
                        statusDurationStr = FormatTime(now - lastStateChangeTime);
                    }
                }
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