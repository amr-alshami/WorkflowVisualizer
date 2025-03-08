namespace WorkflowVisualizer.Models
{
    public class WorkflowNode
    {
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string NodeType { get; private set; }
        public string Details { get; set; }
        public int NodeLevel { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public WorkflowNode(string name, double x, double y, string nodeType, string details, int nodeLevel = 1)
        {
            NodeType = nodeType;
            Y = y;
            X = x;
            NodeLevel = nodeLevel;
            Name = name;
            Details = details;
        }
    }
}
