namespace WorkflowVisualizer.Models
{
    public class WorkflowModel
    {
        public int WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public string Details { get; set; }
        public List<wkfRules> WorkflowRules { get; set; } = new List<wkfRules>();
    }
}
