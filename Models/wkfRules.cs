namespace WorkflowVisualizer.Models
{
    public class wkfRules
    {
        public int RuleId { get; set; }
        public string? RuleName { get; set; }
        public string Details { get; set; }
        public List<WkfActions> Actions { get; set; }
    }
}
