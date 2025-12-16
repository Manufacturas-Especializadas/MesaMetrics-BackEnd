namespace MESAmetrics.Dtos
{
    public class RealTimeDto
    {
        public string Title { get; set; }

        public int? ShiftId { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public int? LineId { get; set; }

        public List<int>? TagsId { get; set; }
    }
}