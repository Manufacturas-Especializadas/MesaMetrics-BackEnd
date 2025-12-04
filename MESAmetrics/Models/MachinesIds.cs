using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class MachinesIds
{
    public int Id { get; set; }

    public string Machine { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RealTime> RealTime { get; set; } = new List<RealTime>();
}