using System;
using System.Collections.Generic;

namespace MESAmetrics.Models;

public partial class Telemetry
{
    public int Id { get; set; }

    public int? CycleCount { get; set; }

    public int? StopButton { get; set; }

    public int? MessageId { get; set; }

    public int? MachineId { get; set; }
}