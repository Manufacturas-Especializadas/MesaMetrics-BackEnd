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

            var timeline = new List<TimelineSegmentDto>();

            if (!logs.Any()) return new MachineMetricsDto();

            bool isSegmentProducing = false;
            if (logs.Count > 1)
            {
                var first = logs[0];
                var second = logs[1];
                var firstDuration = (second.CreatedAt!.Value - first.CreatedAt!.Value).TotalSeconds;
                isSegmentProducing = (firstDuration <= 60) && (second.CycleCount > first.CycleCount);
            }

            var currentSegmentStart = logs[0].CreatedAt!.Value;
            bool wasProducing = false;

            for (int i = 0; i < logs.Count - 1; i++)
            {
                var current = logs[i];
                var next = logs[i + 1];
                var duration = Math.Max(0, (next.CreatedAt!.Value - current.CreatedAt!.Value).TotalSeconds);

                bool isGap = duration > 60;
                bool isIntervalProducing = !isGap && (next.CycleCount > current.CycleCount);

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

                if (isIntervalProducing != isSegmentProducing)
                {
                    AddSegment(timeline, isSegmentProducing ? "produccion" : "detenido", currentSegmentStart, current.CreatedAt!.Value);
                    currentSegmentStart = current.CreatedAt!.Value;
                    isSegmentProducing = isIntervalProducing;
                }
            }

            var lastLog = logs.Last();
            var now = DateTime.Now;
            var timeSinceLastSignal = Math.Max(0, (now - lastLog.CreatedAt!.Value).TotalSeconds);

            string currentStatus = "detenido";
            bool isLiveProducing = false;

            if (timeSinceLastSignal > 60)
            {
                currentStatus = "detenido";
                stopSeconds += timeSinceLastSignal;
                if (wasProducing) stopCount++;

                AddSegment(timeline, isSegmentProducing ? "produccion" : "detenido", currentSegmentStart, lastLog.CreatedAt!.Value);

                AddSegment(timeline, "detenido", lastLog.CreatedAt!.Value, now);
            }
            else
            {
                if (logs.Count >= 2)
                {
                    var penultimate = logs[logs.Count - 2];
                    isLiveProducing = lastLog.CycleCount > penultimate.CycleCount;
                }

                currentStatus = isLiveProducing ? "produccion" : "detenido";

                if (isLiveProducing) productionSeconds += timeSinceLastSignal;
                else stopSeconds += timeSinceLastSignal;

                if (isLiveProducing != isSegmentProducing)
                {
                    AddSegment(timeline, isSegmentProducing ? "produccion" : "detenido", currentSegmentStart, lastLog.CreatedAt!.Value);
                    currentSegmentStart = lastLog.CreatedAt!.Value;
                }

                AddSegment(timeline, isLiveProducing ? "produccion" : "detenido", currentSegmentStart, now);
            }

            double totalTime = productionSeconds + stopSeconds;
            double availability = totalTime > 0 ? (productionSeconds / totalTime) * 100 : 0;

            var lastSeg = timeline.LastOrDefault();
            string statusDurationStr = lastSeg != null ? lastSeg.Duration : "0m";

            return new MachineMetricsDto
            {
                MachineName = session.Title,
                Shift = session.Shift?.ShiftName ?? "N/A",
                Status = currentStatus,
                StatusDuration = statusDurationStr,
                Availability = $"{availability:F1}%",
                ProductionTime = FormatTime(TimeSpan.FromSeconds(productionSeconds)),
                StopTime = FormatTime(TimeSpan.FromSeconds(stopSeconds)),
                Stops = stopCount.ToString(),
                Timeline = timeline
            };
        }

        private void AddSegment(List<TimelineSegmentDto> list, string status, DateTime start, DateTime end)
        {
            var duration = end - start;
            if (duration.TotalSeconds < 1) return;

            list.Add(new TimelineSegmentDto
            {
                Status = status,
                StartTime = start,
                EndTime = end,
                Duration = FormatTime(duration)
            });
        }

        private string FormatTime(TimeSpan ts)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        }
    }
}