namespace WorkflowVisualizer.Models;

public partial class WkfActnCode
{
    public int ActionCde { get; set; }

    public string? ActionDsc { get; set; }

    public string Params { get; set; } = null!;

    public string ActiveInd { get; set; } = null!;

    public string SysInd { get; set; } = null!;

    public DateTime? ExecutionDte { get; set; }

    public string? LanguageCde { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? RecordVer { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }

    public virtual ICollection<WkfActn> WkfActns { get; set; } = new List<WkfActn>();
}
