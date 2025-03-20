using System;
using System.Collections.Generic;

namespace WorkflowVisualizer.Models;

public partial class UserGrupCode
{
    public int UserGroupSeqId { get; set; }

    public string? UserGroupDsc { get; set; }

    public DateTime? ExecutionDte { get; set; }

    public bool? ActiveInd { get; set; }

    public bool? SysInd { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? RecordVer { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }

    public string? RoleCde { get; set; }

    public string UserGrupCde { get; set; } = null!;

    public bool AdminInd { get; set; }

    public bool IsMCollAllow { get; set; }

    public bool CanApprove { get; set; }

    public bool? NatnWrqu { get; set; }

    public bool? CrdtRcmdApprInd { get; set; }
}
