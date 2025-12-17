namespace MESAmetrics.Dtos
{
    public class MachineMetricsDto
    {
        public string MachineName { get; set; }

        public string Shift { get; set; }

        public string Status { get; set; }

        public string StatusDuration { get; set; }

        public string Availability { get; set; }

        public string ProductionTime { get; set; }

        public string StopTime { get; set; }

        public string Stops { get; set; }

        public List<TimelineSegmentDto> Timeline {  get; set; } = new List<TimelineSegmentDto>();
    }
}