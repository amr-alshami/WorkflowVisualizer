namespace WorkflowVisualizer.Models;

public partial class WkfModl
{
    public int ModelCde { get; set; }

    public string? ModelDsc { get; set; }

    public string? Formula { get; set; }

    public string? FormulaDsc { get; set; }

    public string? ActiveInd { get; set; }

    public string? SysInd { get; set; }

    public DateTime? CreatedDte { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ExecutionDte { get; set; }

    public int? ExecutionOffset { get; set; }

    public int? SessionId { get; set; }

    public string? SessionCde { get; set; }

    public short? ExecPriority { get; set; }

    public string? RequestTypeCode { get; set; }

    public virtual ICollection<WkfActn> WkfActns { get; set; } = new List<WkfActn>();
}
