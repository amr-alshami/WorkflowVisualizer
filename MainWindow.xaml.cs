
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
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

        private void ShowDetailsPanel(WorkflowNode node)
        {
           // Resize("Out");
            // Update the details for the selected node
            NodeNameText.Text = node.Name;
            NodeDetailsText.Text = node.Discripton;

            if (!string.IsNullOrEmpty(node.Detail))
            {
                NodeMiscDetails.Visibility = Visibility.Visible;
                lblNodeMisc.Content = getNodeMiscDetailLabelName(node);
                txtNodeMisc.Text = node.Detail;
            }
            else
                NodeMiscDetails.Visibility = Visibility.Collapsed;

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

            // Apply blur effect to all elements except the clicked node
            ApplyBlurEffect(node);
        }

        private string getNodeMiscDetailLabelName(WorkflowNode node)
        {
            var labelName = string.Empty;
            switch (node.Name.ToLower())
            {
                case "forward":
                    labelName = "Forward to: ";
                    break;
                case "change status":
                    labelName = "New Status: ";
                    break;
            }
            return labelName;
        }

        private void CloseDetailsPanel(object sender, RoutedEventArgs e)
        {
            //Resize("In");
            // Slide out the details panel
            var slideOut = new DoubleAnimation(DetailsPanel.Width, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            DetailsTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);

            // Remove blur effect from all elements
            RemoveBlurEffect();
        }

        private void ApplyBlurEffect(WorkflowNode node)
        {
            foreach (var child in WorkflowCanvas.Children)
            {
                if (child is UIElement element && element.Uid is string elementId)
                {
                    if (elementId != node.Id)
                    {
                        ApplyAnimatedBlurEffect(element, 0, 5, 0.3); // Animate blur effect from 0 to 5 over 0.3 seconds
                    }
                    else
                    {
                        ApplyAnimatedBlurEffect(element, 5, 0, 0.3); // Animate blur effect from 5 to 0 over 0.3 seconds
                    }
                }
            }
        }


        private void RemoveBlurEffect()
        {
            foreach (var child in WorkflowCanvas.Children)
            {
                if (child is UIElement element && element.Effect is BlurEffect blurEffect)
                {
                    var animation = new DoubleAnimation
                    {
                        From = blurEffect.Radius,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
                }
            }
        }

        private void TogglePanelSize(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;

            var newWidth = _isExpanded ? 600 : 250;

            txtNodeMisc.MaxWidth = _isExpanded ? 500 : 150;

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
            node.Id = Guid.NewGuid().ToString();
            // Check if an icon exists for the node
            var iconPath = $"pack://application:,,,/Resources/{node.Name}.png";
            var iconUri = new Uri(iconPath, UriKind.Absolute);
            bool iconExists = false;

            try
            {
                var iconStream = Application.GetResourceStream(iconUri);
                if (iconStream != null)
                {
                    iconExists = true;
                }
            }
            catch
            {
                // Icon does not exist
            }

            if (iconExists && node.NodeType != "Rule")
            {
                // Draw node with icon
                var image = new Image
                {
                    Source = new BitmapImage(iconUri),
                    Cursor = Cursors.Hand,
                    ToolTip = node.Name,
                    Uid = node.Id // Set Uid to node's Id
                };

                setImageSize(image, node.Name.ToLower());

                Canvas.SetLeft(image, node.X - image.Width / 2);
                Canvas.SetTop(image, node.Y - image.Height / 2);

                image.MouseLeftButtonDown += (s, e) => ShowDetailsPanel(node);

                WorkflowCanvas.Children.Add(image);

                if (isAnimate)
                {
                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    image.BeginAnimation(OpacityProperty, fadeIn);
                }
            }
            else
            {
                // Draw default rule node with rule icon
                var ruleIconPath = "pack://application:,,,/Resources/rule.png";
                var ruleIconUri = new Uri(ruleIconPath, UriKind.Absolute);

                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Background = Brushes.White,
                    Cursor = Cursors.Hand,
                    ToolTip = node.Discripton
                };

                var icon = new Image
                {
                    Source = new BitmapImage(ruleIconUri),
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(-20, -25, 5, -25)
                };

                var textBlock = new TextBlock
                {
                    Text = node.Name,
                    FontWeight = FontWeights.Medium,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(-2, 1, 8, 1)
                };

                stackPanel.Children.Add(icon);
                stackPanel.Children.Add(textBlock);

                // Create a Border with rounded corners
                var border = new Border
                {
                    Uid = node.Id, // Set Uid to node's Id
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    Background = Brushes.White,
                    Padding = new Thickness(2),
                    Child = stackPanel
                };

                // Force a layout update to calculate ActualWidth and ActualHeight
                border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                border.Arrange(new Rect(border.DesiredSize));

                // Calculate the position of the border
                double borderWidth = border.ActualWidth;
                double borderHeight = border.ActualHeight;

                Canvas.SetLeft(border, node.X - borderWidth / 2);
                Canvas.SetTop(border, node.Y - borderHeight / 2);

                border.MouseLeftButtonDown += (s, e) => ShowDetailsPanel(node);

                WorkflowCanvas.Children.Add(border);

                if (isAnimate)
                {
                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    border.BeginAnimation(OpacityProperty, fadeIn);
                }
            }
        }
        private void setImageSize(Image image, string name)
        {
            switch (name)
            {
                case "start":
                    image.Width = 60;
                    image.Height = 60;
                    break;
                case "end":
                    image.Width = 40;
                    image.Height = 40;
                    break;
                case "auto approve":
                    image.Width = 60;
                    image.Height = 60;
                    break;
                default:
                    image.Width = 80;
                    image.Height = 80;
                    break;
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


        //private void Resize(string type)
        //{
        //    if (type == "Out")
        //    {
        //        WorkflowCanvas.Margin = new Thickness(200, 0, 250, 0);
        //    }
        //    else
        //    {
        //        WorkflowCanvas.Margin = new Thickness(200, 0, 0, 0);
        //    }

        //    // Force a layout update to refresh the ActualWidth property

        //    WorkflowCanvas.UpdateLayout();
           
        //    refreshView();
        //}

        //private void refreshView()
        //{
        //      // Load the selected workflow details
        //    _viewModel.ApplyHierarchicalLayout();

        //    // Draw the workflow graph
        //    DrawGraph(false);
        //}
        private void ApplyAnimatedBlurEffect(UIElement element, double fromRadius, double toRadius, double durationInSeconds)
        {
            var blurEffect = new BlurEffect();
            element.Effect = blurEffect;

            var animation = new DoubleAnimation
            {
                From = fromRadius,
                To = toRadius,
                Duration = TimeSpan.FromSeconds(durationInSeconds),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
        }
    }
}