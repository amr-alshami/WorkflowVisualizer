namespace WorkflowVisualizer.Models.Custom
{
    public class wkfRules
    {
        public int RuleId { get; set; }
        public string? RuleName { get; set; }
        public string Discrption { get; set; }
        public string Detail { get; set; }
        public List<WkfActions> Actions { get; set; }
    }
}
