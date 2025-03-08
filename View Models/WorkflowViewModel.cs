using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using WorkflowVisualizer.Data;
using WorkflowVisualizer.Models;

namespace WorkflowVisualizer.ViewModels
{
    public class WorkflowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WorkflowNode> Nodes { get; set; }
        public ObservableCollection<WorkflowEdge> Edges { get; set; }
        public ObservableCollection<WorkflowModel> Workflows { get; set; }
        public ICollectionView FilteredWorkflows { get; set; } // Expose the filtered collection

        private Canvas _workflowCanvas;
        private List<WkfRule> _wkfRules;
        private List<WkfActnCode> _wkfActionCodes;

        public WorkflowViewModel(Canvas workflowCanvas)
        {
            Nodes = new ObservableCollection<WorkflowNode>();
            Edges = new ObservableCollection<WorkflowEdge>();
            Workflows = new ObservableCollection<WorkflowModel>();

            _workflowCanvas = workflowCanvas;

            // Load the Data asynchronously
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                using (var context = new WkfDbContext())
                {
                    var wkfModls = await context.WkfModls.ToListAsync();
                    foreach (var wkf in wkfModls)
                    {
                        Workflows.Add(new WorkflowModel { WorkflowId = wkf.ModelCde, WorkflowName = wkf.ModelDsc });
                    }
                    _wkfRules = await context.WkfRules.ToListAsync();
                    _wkfActionCodes = await context.WkfActnCodes.ToListAsync();
                }

                // Wrap the Workflows collection in a CollectionViewSource
                FilteredWorkflows = CollectionViewSource.GetDefaultView(Workflows);
                OnPropertyChanged(nameof(FilteredWorkflows));
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public async Task GetWorkflowDetails(WorkflowModel workflow)
        {
            try
            {
                workflow.Details = "XYZ"; // ToDo: from new misc table
                var _wkfActns = await GetWorkflowActions(workflow.WorkflowId);
                foreach (var actn in _wkfActns)
                {
                    if (!workflow.WorkflowRules.Any(t => t.RuleId == actn.RuleId))
                    {
                        workflow.WorkflowRules.Add(new wkfRules
                        {
                            RuleId = actn.RuleId ?? 0,
                            RuleName = _wkfRules.FirstOrDefault(t => t.RuleId == actn.RuleId)?.RuleDsc,
                            Details = "XYZ", // ToDo: from new misc table
                            Actions = new List<WkfActions>()
                        });
                    }

                    if (workflow.WorkflowRules.Any(t => t.RuleId == actn.RuleId && t.Actions != null && !t.Actions.Any(t => t.ActionCode == actn.ActionCde && t.ActionSequence == actn.ExeSequence)))
                    {
                        workflow.WorkflowRules.FirstOrDefault(t => t.RuleId == actn.RuleId)?.Actions?.Add(new WkfActions
                        {
                            ActionCode = actn.ActionCde ?? 0,
                            ActionName = _wkfActionCodes.FirstOrDefault(t => t.ActionCde == actn.ActionCde)?.ActionDsc,
                            ActionSequence = actn.ExeSequence ?? 0,
                            Details = "XYZ", // ToDo: from new misc table
                        });
                    }
                }

                // Create nodes and edges based on the workflow rules and actions
                CreateNodesAndEdges(workflow);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error getting workflow details: {ex.Message}");
            }
        }

        public void FilterWorkflows(string searchText)
        {
            // Apply the filter
            FilteredWorkflows.Filter = item =>
            {
                var workflow = item as WorkflowModel;
                return workflow.WorkflowName.ToLower().Contains(searchText.ToLower());
            };
        }

        private void CreateNodesAndEdges(WorkflowModel workflow)
        {
            Nodes.Clear();
            Edges.Clear();

            // Add Start Node
            var startNode = new WorkflowNode("Start", 0, 0, "Start", "Start Node");
            Nodes.Add(startNode);

            // Add Rule Nodes as children of the Start Node
            double yOffset = 100; // Vertical spacing for nodes
            double xOffset = 150; // Horizontal spacing for nodes

            foreach (var rule in workflow.WorkflowRules)
            {
                var ruleNode = new WorkflowNode(rule.RuleName, 0, 0, "Rule", rule.Details);
                Nodes.Add(ruleNode);

                // Connect Start Node to Rule Node
                Edges.Add(new WorkflowEdge(startNode, ruleNode));

                // Add Action Nodes in sequence under the Rule Node
                if (rule.Actions != null)
                {
                    var sortedActions = rule.Actions.OrderBy(a => a.ActionSequence).ToList();
                    WorkflowNode previousActionNode = null;

                    for (int i = 0; i < sortedActions.Count; i++)
                    {
                        var action = sortedActions[i];
                        var actionNode = new WorkflowNode(action.ActionName, 0, 0, "Action", action.Details, action.ActionSequence);
                        Nodes.Add(actionNode);

                        // Position action nodes sequentially
                        actionNode.X = ruleNode.X + xOffset;
                        actionNode.Y = ruleNode.Y + (i + 1) * yOffset;

                        // Connect Rule Node to the first Action Node
                        if (i == 0)
                        {
                            Edges.Add(new WorkflowEdge(ruleNode, actionNode));
                        }

                        // Connect previous Action Node to current Action Node
                        if (previousActionNode != null)
                        {
                            Edges.Add(new WorkflowEdge(previousActionNode, actionNode));
                        }

                        previousActionNode = actionNode;
                    }
                }
            }

            // Add End Node
            var endNode = new WorkflowNode("End", 0, 0, "End", "End Node");
            Nodes.Add(endNode);

            // Connect all last-level Action Nodes to the End Node
            var lastActionNodes = Nodes
                .Where(n => n.NodeType == "Action" && !Edges.Any(e => e.From == n))
                .ToList();

            foreach (var lastActionNode in lastActionNodes)
            {
                Edges.Add(new WorkflowEdge(lastActionNode, endNode));
            }

            // Apply the layout algorithm
            ApplyHierarchicalLayout();
        }

        private void ApplyHierarchicalLayout()
        {
            // Get the number of levels (maximum depth of the graph)
            int maxLevel = Nodes.Max(n => n.NodeLevel) + 2; // +2 to include the Start node and end node

            // Calculate dynamic spacing
            double canvasHeight = _workflowCanvas.ActualHeight;
            double canvasWidth = _workflowCanvas.ActualWidth;

            double levelSpacing = canvasHeight / (maxLevel + 1); // Evenly distribute vertical space
            double nodeSpacing = canvasWidth / (Nodes.Count(n => n.NodeType == "Rule") + 1); // Evenly distribute horizontal space

            // Position Start Node
            var startNode = Nodes.First(n => n.NodeType == "Start");
            startNode.X = canvasWidth / 2; // Center horizontally
            startNode.Y = levelSpacing / 2; // First level

            // Position Rule Nodes
            var ruleNodes = Nodes.Where(n => n.NodeType == "Rule").ToList();
            for (int i = 0; i < ruleNodes.Count; i++)
            {
                ruleNodes[i].X = (i + 1) * nodeSpacing; // Distribute horizontally
                ruleNodes[i].Y = startNode.Y + levelSpacing; // Second level
            }

            // Position Action Nodes
            var actionNodes = Nodes.Where(n => n.NodeType == "Action").ToList();
            foreach (var ruleNode in ruleNodes)
            {
                var connectedActions = Edges
                    .Where(e => e.From == ruleNode)
                    .Select(e => e.To)
                    .ToList();

                for (int i = 0; i < connectedActions.Count; i++)
                {
                    connectedActions[i].X = ruleNode.X; // Align under the rule node
                    connectedActions[i].Y = ruleNode.Y + (i + 1) * levelSpacing; // Distribute vertically
                }
            }

            // Position Action childs (level > 1)
            foreach (var actionNode in actionNodes.Where(s => s.NodeLevel > 0))
            {
                var connectedActions = Edges
                    .Where(e => e.From == actionNode)
                    .Select(e => e.To)
                    .ToList();

                for (int i = 0; i < connectedActions?.Count; i++)
                {
                    connectedActions[i].X = actionNode.X + (i * nodeSpacing);
                    connectedActions[i].Y = actionNode.Y + (i + 1) * levelSpacing;
                }
            }

            // Position End Node
            var endNode = Nodes.First(n => n.NodeType == "End");
            endNode.X = canvasWidth / 2; // Center horizontally
            endNode.Y = actionNodes.Max(t => t.Y) + levelSpacing; // Last level
        }

        private async Task<List<WkfActn>> GetWorkflowActions(int workflowId)
        {
            using (var context = new WkfDbContext())
            {
                return await context.WkfActns.Where(t => t.ModelCde == workflowId).ToListAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}