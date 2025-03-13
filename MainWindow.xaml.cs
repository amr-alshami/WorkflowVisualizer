
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WorkflowVisualizer.Models.Custom;
using WorkflowVisualizer.ViewModels;

namespace WorkflowVisualizer
{
    public partial class MainWindow : Window
    {
        private WorkflowViewModel _viewModel;
        private WorkflowModel _model;
        private bool _isExpanded = false;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new WorkflowViewModel(WorkflowCanvas);
            DataContext = _viewModel; // Set DataContext to the ViewModel

            // Bind the ListView to the filtered collection
            WorkflowListView.ItemsSource = _viewModel.FilteredWorkflows;
        }

        // Handle search box text changes
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Show or hide the placeholder text based on whether the TextBox is empty
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            // Get the search text
            string searchText = SearchBox.Text.ToLower();

            // Apply the filter
            _viewModel.FilterWorkflows(searchText);
        }

        // Handle workflow selection
        private async void WorkflowListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkflowListView.SelectedItem is WorkflowModel selectedWorkflow)
            {
                _model = selectedWorkflow;
                // Clear existing nodes and edges
                _viewModel.Nodes.Clear();
                _viewModel.Edges.Clear();

                // Load the selected workflow details
                await _viewModel.GetWorkflowDetails(_model);

                // Draw the workflow graph
                DrawGraph(true);
            }
        }


        private void ShowDetailsPanel(WorkflowNode node, Shape selectedNode)
        {
            Resize("Out");
            // Update the details for the selected node
            NodeNameText.Text = node.Name;
            NodeDetailsText.Text = node.Details;

            // Fade-in text effect
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            NodeNameText.BeginAnimation(OpacityProperty, fadeIn);
            NodeDetailsText.BeginAnimation(OpacityProperty, fadeIn);

            // Animate panel opening
            var slideIn = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            DetailsTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
        }

        private void CloseDetailsPanel(object sender, RoutedEventArgs e)
        {
            Resize("In");
            // Slide out the details panel
            var slideOut = new DoubleAnimation(DetailsPanel.Width, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            DetailsTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }

        private void TogglePanelSize(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;

            var newWidth = _isExpanded ? 600 : 250;
            var animation = new DoubleAnimation(newWidth, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DetailsPanel.BeginAnimation(WidthProperty, animation);
            ExpandCollapseButton.Content = _isExpanded ? "↪ Collapse" : "↩ Expand";
        }

        private void DrawGraph(bool isAnimate = true)
        {
            WorkflowCanvas.Children.Clear();

            // Draw edges (connections first so they appear below nodes)
            foreach (var edge in _viewModel.Edges)
            {
                DrawEdge(edge, isAnimate);
            }

            // Draw nodes
            foreach (var node in _viewModel.Nodes)
            {
                DrawNode(node, isAnimate);
            }
        }

        private void DrawNode(WorkflowNode node, bool isAnimate = true)
        {
            // Measure the text size
            var textBlock = new TextBlock
            {
                Text = node.Name,
                FontWeight = FontWeights.Medium,
                FontSize = 12,
                Foreground = (node.NodeType == "Start" || node.NodeType == "End") ? Brushes.White : Brushes.Black,
                TextAlignment = TextAlignment.Center,
                Cursor = Cursors.Hand
            };

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            double textWidth = textBlock.DesiredSize.Width;
            double textHeight = textBlock.DesiredSize.Height;

            // Define node size and shape
            node.Width = textWidth + 20; // Add padding
            node.Height = textHeight + 10; // Add padding

            Shape shape;

            if (node.NodeType == "Start" || node.NodeType == "End")
            {
                node.Width = 40;
                node.Height = 35;
                // Draw Start and End nodes as circles
                shape = new Ellipse
                {
                    Width = node.Width,
                    Height = node.Height, // Circle: width = height
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Cursor = Cursors.Hand
                };

                // Apply styles based on NodeType
                shape.Fill = node.NodeType == "Start" ? Brushes.Green : Brushes.Red;
            }
            else
            {
                // Draw Rule and Action nodes as rectangles
                shape = new Rectangle
                {
                    Width = node.Width,
                    Height = node.Height,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Cursor = Cursors.Hand
                };

                // Apply styles based on NodeType
                switch (node.NodeType)
                {
                    case "Rule":
                        shape.Fill = Brushes.LightBlue;
                        break;
                    case "Action":
                        shape.Fill = Brushes.LightYellow;
                        ((Rectangle)shape).RadiusX = 5;
                        ((Rectangle)shape).RadiusY = 5;
                        break;
                    default:
                        shape.Fill = Brushes.LightGray;
                        break;
                }
            }

            Canvas.SetLeft(shape, node.X - node.Width / 2); // Center the node horizontally
            Canvas.SetTop(shape, node.Y - node.Height / 2); // Center the node vertically

            // Add text to the rectangle
            Canvas.SetLeft(textBlock, node.X - textWidth / 2); // Center text horizontally
            Canvas.SetTop(textBlock, node.Y - textHeight / 2); // Center text vertically

            // Handle click event to show details panel
            shape.MouseLeftButtonDown += (s, e) => ShowDetailsPanel(node, shape);
            textBlock.MouseLeftButtonDown += (s, e) => ShowDetailsPanel(node, shape);

            WorkflowCanvas.Children.Add(shape);
            WorkflowCanvas.Children.Add(textBlock);

            if (isAnimate)
            {
                // Animate the node
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                shape.BeginAnimation(OpacityProperty, fadeIn);
                textBlock.BeginAnimation(OpacityProperty, fadeIn);

                var translateTransform = new TranslateTransform();
                shape.RenderTransform = translateTransform;
                textBlock.RenderTransform = translateTransform;

                var slideIn = new DoubleAnimation(-5, 0, TimeSpan.FromSeconds(0.5))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                translateTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
            }
        }

        private void DrawEdge(WorkflowEdge edge, bool isAnimate = true)
        {
            // Calculate the start and end points of the line
            Point startPoint = new Point(edge.From.X, edge.From.Y); // Center of source node
            Point endPoint = new Point(edge.To.X, edge.To.Y); // Center of destination node

            // Draw the main line
            var line = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = startPoint.X, // Start from the same point
                Y2 = startPoint.Y, // Start from the same point
                Stroke = Brushes.Black,
                StrokeThickness = 0.8,
                StrokeDashArray = new DoubleCollection { 2, 2 } // Dotted line pattern
            };


            if (isAnimate)
            {

                WorkflowCanvas.Children.Add(line);
                // Animate the line drawing
                var x2Animation = new DoubleAnimation(startPoint.X, endPoint.X, TimeSpan.FromSeconds(0.5));
                var y2Animation = new DoubleAnimation(startPoint.Y, endPoint.Y, TimeSpan.FromSeconds(0.5));

                line.BeginAnimation(Line.X2Property, x2Animation);
                line.BeginAnimation(Line.Y2Property, y2Animation);
            }
            else
            {
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
                WorkflowCanvas.Children.Add(line);
            }

            // Draw the arrowhead at the midpoint of the line
            DrawArrowhead(startPoint, endPoint);
        }

        private void DrawArrowhead(Point startPoint, Point endPoint)
        {
            // Calculate the midpoint of the line
            double midX = (startPoint.X + endPoint.X) / 2;
            double midY = (startPoint.Y + endPoint.Y) / 2;
            Point midpoint = new Point(midX, midY);

            // Calculate the angle of the line in radians
            double angle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);

            // Define the arrowhead size
            double arrowheadSize = 6;

            // Calculate the arrowhead points
            Point arrowPoint1 = new Point(
                midpoint.X - arrowheadSize * Math.Cos(angle - Math.PI / 6),
                midpoint.Y - arrowheadSize * Math.Sin(angle - Math.PI / 6)
            );

            Point arrowPoint2 = new Point(
                midpoint.X - arrowheadSize * Math.Cos(angle + Math.PI / 6),
                midpoint.Y - arrowheadSize * Math.Sin(angle + Math.PI / 6)
            );

            // Create the arrowhead as a Polygon
            var arrowhead = new Polygon
            {
                Points = new PointCollection { midpoint, arrowPoint1, arrowPoint2 },
                Fill = Brushes.Black,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            WorkflowCanvas.Children.Add(arrowhead);
        }
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            // Logic to zoom in on the canvas
            var scaleTransform = new ScaleTransform(1.2, 1.2);
            WorkflowCanvas.LayoutTransform = scaleTransform;
            refreshView();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            // Logic to zoom out on the canvas
            var scaleTransform = new ScaleTransform(0.8, 0.8);
            WorkflowCanvas.LayoutTransform = scaleTransform;
            // Force a layout update to refresh the ActualWidth property
            WorkflowCanvas.UpdateLayout();

            refreshView();
        }

        private void Resize(string type)
        {
            if (type == "Out")
            {
                WorkflowCanvas.Margin = new Thickness(200, 0, 200, 0);
            }
            else
            {
                WorkflowCanvas.Margin = new Thickness(200, 0, 0, 0);
            }

            // Force a layout update to refresh the ActualWidth property
            WorkflowCanvas.UpdateLayout();

            refreshView();
        }

        private void refreshView()
        {
            // Clear existing nodes and edges
            _viewModel.Nodes.Clear();
            _viewModel.Edges.Clear();

            // Load the selected workflow details
            _viewModel.CreateNodesAndEdges(_model);

            // Draw the workflow graph
            DrawGraph(false);
        }
    }
}