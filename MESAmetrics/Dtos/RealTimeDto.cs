namespace MESAmetrics.Dtos
{
    public class RealTimeDto
    {
        public string Title { get; set; }

        public int? ShiftId { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public int? TagsId { get; set; }
    }
}