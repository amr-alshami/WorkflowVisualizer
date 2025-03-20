using System;
using System.Collections.Generic;

namespace WorkflowVisualizer.Models;

public partial class StusCode
{
    public string StatusCde { get; set; } = null!;

    public string? StatusDsc { get; set; }

    public bool? ActiveInd { get; set; }

    public bool? SysInd { get; set; }

    public DateTime? ExecutionDte { get; set; }

    public string? LanguageCde { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? RecordVer { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }

    public int StatusSeqId { get; set; }
}
