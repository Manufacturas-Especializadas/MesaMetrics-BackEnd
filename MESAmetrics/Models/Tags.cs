using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class Tags
{
    public int Id { get; set; }

    public string TagsName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RealTimeTags> RealTimeTags { get; set; } = new List<RealTimeTags>();
}