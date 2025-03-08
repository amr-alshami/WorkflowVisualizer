# WorkflowVisualizer

Workflow Visualizer is a WPF application designed to visualize workflows. It provides an interactive interface to load, filter, and display workflows, including their nodes and edges, in a hierarchical layout. The application leverages Entity Framework Core for data access and supports .NET 8.

## Features

- **Workflow Visualization**: Visualize workflows with nodes and edges representing rules and actions.
- **Interactive UI**: Click on nodes to view detailed information in an animated details panel.
- **Search and Filter**: Easily search and filter workflows using a search box.
- **Dynamic Layout**: Automatically arrange nodes in a hierarchical layout for clear visualization.
- **Asynchronous Data Loading**: Load workflow data asynchronously from a SQL Server database.

## Technologies Used

- **.NET 8**
- **C# 12.0**
- **WPF (Windows Presentation Foundation)**
- **Entity Framework Core 9.0.2**
- **SQL Server**

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server

### Installation

1. Clone the repository:
git clone https://github.com/your-username/workflow-visualizer.git
cd workflow-visualizer

2. Set up the database connection:
    - Update the connection string in `appsettings.json` or use User Secrets to store your connection string securely.

3. Apply database migrations:
    dotnet ef database update
    
4. Run the application:
    dotnet run

    
## Project Structure

- **MainWindow.xaml.cs**: The main window code-behind file that handles UI interactions and drawing the workflow graph.
- **ViewModels/WorkflowViewModel.cs**: The ViewModel class that manages workflow data, filtering, and node/edge creation.
- **Models**: Contains model classes representing database entities such as `WkfActn`, `WkfActnCode`, `WkfRule`, and `WorkflowNode`.
- **WorkflowVisualizer.csproj**: The project file with dependencies and build configurations.

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests for any enhancements or bug fixes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [Microsoft Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [WPF (Windows Presentation Foundation)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
