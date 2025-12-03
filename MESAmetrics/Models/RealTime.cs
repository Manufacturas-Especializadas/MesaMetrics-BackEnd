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

    public int? Availability { get; set; }

    public int? ProductionTime { get; set; }

    public int? EvenTime { get; set; }

    public int? Strikes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RealTimeTags> RealTimeTags { get; set; } = new List<RealTimeTags>();

    public virtual Shifts Shift { get; set; }

    public virtual ICollection<Telemetry> Telemetry { get; set; } = new List<Telemetry>();
}