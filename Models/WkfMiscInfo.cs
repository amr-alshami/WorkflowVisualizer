using System;
using System.Collections.Generic;

namespace WorkflowVisualizer.Models;

public partial class WkfMiscInfo
{
    public int MiscInfoId { get; set; }

    public string? RefType { get; set; }

    public int? RefId { get; set; }

    public string? Detail { get; set; }
}
