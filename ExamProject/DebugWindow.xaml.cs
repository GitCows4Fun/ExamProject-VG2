using System.Windows;
using System.Windows.Controls;
using ExamProject;
using Newtonsoft.Json.Linq;
using System.Windows.Input;

namespace ExamProject
{
	public partial class DebugWindow : Window
	{
		private ViewModel _dataHandler;

		public DebugWindow()
		{
			InitializeComponent();
            this.KeyUp += KeyPressHandler;

            _dataHandler = new ViewModel();
			DataContext = _dataHandler;
			_dataHandler.InitializeViews(DataGrid);

            // Load initial JSON data and populate the TreeView
            JArray initialData = JsonParser.ReadJsonFromFile(JSON_FILE_PATH.upgrades);
            TreeViewItem rootItem = CreateRootNode(initialData);
            TreeView.Items.Add(rootItem);
            _dataHandler.UpdateTreeView(initialData, rootItem);
        }

		private void KeyPressHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
				Close();
            }
        }

        private TreeViewItem CreateRootNode(JToken node)
		{
			TreeViewItem rootNode = new TreeViewItem();
			rootNode.Header = "Root";
			rootNode.Tag = node;
			return rootNode;
		}

		private void CommandInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null) return;

			if (textBox.Text.EndsWith("\n") || textBox.Text.EndsWith("\r\n"))
			{
				string command = textBox.Text.Trim();
				ProcessCommand(command);
				AddToCommandHistory(command);
				textBox.Text = "";
			}
		}

		private void ProcessCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command)) return;

			switch (command.ToLower())
			{
				case "help":
					ShowHelp();
					break;
				case "refresh":
					_dataHandler.LoadJsonData(JSON_FILE_PATH.upgrades);
					break;
				case "back":
					if (_dataHandler.NavigationHistory.Count > 0)
					{
						var previousNode = _dataHandler.NavigationHistory.Pop();
						_dataHandler.UpdateTreeView((JToken)previousNode);
						_dataHandler.UpdateDataGrid((JToken)previousNode);
					}
					break;
				default:
					MessageBox.Show("Unknown command. Type 'help' for available commands.");
					break;
			}
		}

		private void AddToCommandHistory(string command)
		{
			CommandHistory.Items.Add($"{DateTime.Now:HH:mm:ss} > {command}");
			CommandHistory.SelectedIndex = CommandHistory.Items.Count - 1;
		}

		private void ShowHelp()
		{
			string helpText = "Available Commands:\n" +
							"- help: Displays available commands.\n" +
							"- refresh: Reloads the JSON data.\n" +
							"- back: Navigates back to the previous view.";
			MessageBox.Show(helpText, "Command Help");
		}

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedTreeViewItem = sender as TreeViewItem;
            if (selectedTreeViewItem == null) return;

            // Get the current node from the TreeViewItem's DataContext
            if (selectedTreeViewItem.DataContext is JToken currentNode)
			{
				// Push the previous node to the navigation history
				if (_dataHandler.CurrentSelectedItem != null)
				{
					_dataHandler.NavigationHistory.Push(_dataHandler.CurrentSelectedItem.DataContext as JToken);
				}

				// Update the current selected item in DataHandler
				_dataHandler.CurrentSelectedItem = selectedTreeViewItem;

				// Update the TreeView and DataGrid with the current JToken
				_dataHandler.UpdateTreeView(currentNode, selectedTreeViewItem);
				_dataHandler.UpdateDataGrid(currentNode);
			}
		}
	}
}