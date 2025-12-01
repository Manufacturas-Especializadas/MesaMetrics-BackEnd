using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class RealTime
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int? ShiftId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? TagsId { get; set; }

    public int? Availability { get; set; }

    public TimeOnly? ProductionTime { get; set; }

    public TimeOnly? EvenTime { get; set; }

    public int? Strikes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Shifts Shift { get; set; }

    public virtual Tags Tags { get; set; }
}