namespace WorkflowVisualizer.Models
{
    public class WorkflowEdge
    {
        public WorkflowNode From { get; set; }
        public WorkflowNode To { get; set; }

        public WorkflowEdge(WorkflowNode from, WorkflowNode to)
        {
            From = from; To = to;
        }
    }
}
