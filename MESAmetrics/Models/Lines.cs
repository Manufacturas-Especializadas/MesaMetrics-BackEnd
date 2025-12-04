using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class Lines
{
    public int Id { get; set; }

    public string LinesName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RealTime> RealTime { get; set; } = new List<RealTime>();
}