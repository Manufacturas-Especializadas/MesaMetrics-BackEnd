using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class Shifts
{
    public int Id { get; set; }

    public string ShiftName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RealTime> RealTime { get; set; } = new List<RealTime>();
}