using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class RealTimeTags
{
    public int Id { get; set; }

    public int RealTimeId { get; set; }

    public int TagId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual RealTime RealTime { get; set; }

    public virtual Tags Tag { get; set; }
}