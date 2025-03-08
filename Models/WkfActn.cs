namespace WorkflowVisualizer.Models;

public partial class WkfActn
{
    public int ActionId { get; set; }

    public int? RuleId { get; set; }

    public int? ActionCde { get; set; }

    public int? ExeSequence { get; set; }

    public string? Params { get; set; }

    public DateTime? ExecutionDte { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }

    public int StateCde { get; set; }

    public int ModelCde { get; set; }

    public virtual WkfActnCode? ActionCdeNavigation { get; set; }

    public virtual WkfModl ModelCdeNavigation { get; set; } = null!;

    public virtual WkfRule? Rule { get; set; }
}
