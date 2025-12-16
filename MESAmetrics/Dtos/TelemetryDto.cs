namespace MESAmetrics.Dtos
{
    public class TelemetryDto
    {
        public int? RealTimeId { get; set; }

        public int? CycleCount { get; set; }

        public int? StopButton { get; set; }

        public int? MessageId { get; set; }

        public bool? Active { get; set; }
    }
}