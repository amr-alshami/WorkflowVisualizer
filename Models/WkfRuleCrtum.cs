namespace WorkflowVisualizer.Models;

public partial class WkfRuleCrtum
{
    public int ModelCde { get; set; }

    public int StateCde { get; set; }

    public int RuleCde { get; set; }

    public int RuleCdeSeq { get; set; }

    public int? ExpressionSeq { get; set; }

    public string? PrefixExpression { get; set; }

    public string? BusinessRuleCde { get; set; }

    public string? PostfixExpression { get; set; }

    public string? AssociationTyp { get; set; }

    public string? ActiveInd { get; set; }

    public DateTime? ExecutionDte { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }
}
