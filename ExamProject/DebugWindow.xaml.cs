using System.Windows;
using System.Windows.Controls;
using ExamProject;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace ExamProject
{
	public partial class DebugWindow : Window
	{
		private ViewModel _dataHandler;

		public DebugWindow()
		{
			InitializeComponent();
			KeyUp += KeyPressHandler;

			_dataHandler = new ViewModel();
			DataContext = _dataHandler;

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

			string[] commandParts = command.ToLower().Split(" ");

			switch (commandParts[0])
			{
				case "help":
					ShowHelp();
					break;
				case "exit":
					Close();
					break;
				case "refresh":
					_dataHandler.LoadJsonData(JSON_FILE_PATH.currentTreeviewLoaded);
					break;
				case "load":
					{
						if (commandParts.Length < 2)
						{
							ShowLoadOptions();
							break;
						}
						switch (commandParts[1])
						{
							case "json":
								{
									if (commandParts.Length < 3)
										{
											ShowJSONLoadOptions();
											break;
										}
									switch (commandParts[2])
									{
										case "upgrades":
											{
												try
												{
													_dataHandler.LoadJsonData(JSON_FILE_PATH.upgrades);
													break;
												}
												catch (Exception ex)
												{
													MessageBox.Show($"An error occurred: {ex.Message}");
													break;
												}
											}
										case "gd":
											{
												try
												{
													_dataHandler.LoadJsonData(JSON_FILE_PATH.gameData);
												break;
												}
												catch (Exception ex)
												{
												MessageBox.Show($"An error occurred: {ex.Message}");
												break;
												}
									}
										default:
											{
												ShowJSONLoadOptions();
												break;
											}
									}
									break;
								}
							case "db":
								{
									break;
								}
							default:
								{
									ShowLoadOptions();
									break;
								}
						}
						break;
					}
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
							"-\t help: Displays available commands.\n" +
							"-\t refresh: Reloads the JSON data.\n" +
							"-\t load: Load data from JSON files or from a MongoDB Server\n";
			MessageBox.Show(helpText, "Command Help");
		}
		private void ShowLoadOptions()
		{
			string listOptions = "Available Load Options:\n" +
				"- json: Load a JSON file.\n";
			MessageBox.Show(listOptions, "Load Options");
		}
		private void ShowJSONLoadOptions()
		{
			string jsonListOptions = "Available JSON Load Options:\n" +
				"-\t upgrades: Load the upgrades JSON file.\n" +
				"-\t gd: Load the gd JSON file.\n";
			MessageBox.Show(jsonListOptions, "JSON Load Options");
		}
	}
}