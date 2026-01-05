using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class Roles
{
    public int Id { get; set; }

    public string RoleName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
}