using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class MachineStops
{
    public int Id { get; set; }

    public int RealTimeId { get; set; }

    public TimeOnly StopTime { get; set; }

    public int? DurationMinutes { get; set; }

    public string Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual RealTime RealTime { get; set; }
}