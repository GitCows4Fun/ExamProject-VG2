using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExamProject
{
	public class JSON_FILE_PATH
	{
#pragma warning disable IDE1006
		public static string saveLocation { get; } = @"../../../data/saves/";
		public static string upgrades { get; } = @"../../../data/upgrades.json";
		public static string gameData { get; } = @"../../../data/gd.json";
		public static string currentTreeviewLoaded { get; set; } = upgrades;
#pragma warning restore IDE1006
	}

	public static class JsonParser
	{
		public static JArray ReadJsonFromFile(string filePath)
		{
			try
			{
				string json = File.ReadAllText(filePath);
				return JArray.Parse(json);
			}
			catch (FileNotFoundException ex)
			{
				throw new FileNotFoundException("The JSON file was not found.", ex);
			}
			catch (JsonException ex)
			{
				throw new ArgumentException("The file does not contain valid JSON data.", ex);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while reading the JSON file.", ex);
			}
		}

		public static Dictionary<string, object> ParseJsonObject(JObject obj)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			foreach (var prop in obj.Properties())
			{
				dict[prop.Name] = prop.Value.ToObject<object>();
			}
			return dict;
		}

		public static List<Dictionary<string, object>> ParseJArray(JArray array)
		{
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (JObject obj in array)
			{
				list.Add(ParseJsonObject(obj));
			}
			return list;
		}

		public static void SaveJsonToFile(JArray jsonData)
		{
			try
			{
				string json = jsonData.ToString(Formatting.Indented);
				File.WriteAllText("", json);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while saving the JSON file.", ex);
			}
		}
	}

	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private List<Dictionary<string, object>> _jsonData;
		public List<Dictionary<string, object>> JsonData
		{
			get => _jsonData;
			set
			{
				_jsonData = value;
				OnPropertyChanged(nameof(JsonData));
			}
		}
		private Stack<object> _navigationHistory = new Stack<object>();

		public Stack<object> NavigationHistory { get => _navigationHistory; }

		public void LoadJsonData(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException("The JSON file was not found.");
				}

				JArray jsonData = JsonParser.ReadJsonFromFile(filePath);
				JsonData = JsonParser.ParseJArray(jsonData);
				JSON_FILE_PATH.currentTreeviewLoaded = filePath;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred: {ex.Message}");
			}
		}

		public void UpdateTreeView(JToken node, TreeViewItem parent = null)
		{
			if (node == null || parent == null)
			{
				return;
			}

			if (node is JObject)
			{
				foreach (var prop in ((JObject)node).Properties())
				{
					TreeViewItem propItem = new TreeViewItem();
					propItem.Header = $"{prop.Name}: {prop.Value}";
					propItem.Tag = prop.Value;
					parent.Items.Add(propItem);
					UpdateTreeView(prop.Value, propItem);
				}
			}
			else if (node is JArray)
			{
				for (int i = 0; i < ((JArray)node).Count; i++)
				{
					JToken item = ((JArray)node)[i];
					TreeViewItem itemNode = new TreeViewItem();
					itemNode.Header = $"Item {i}";
					itemNode.Tag = item;
					parent.Items.Add(itemNode);

					// Add child nodes for the item's properties
					if (item is JObject itemObj)
					{
						foreach (var prop in itemObj.Properties())
						{
							TreeViewItem propItem = new TreeViewItem();
							propItem.Header = $"{prop.Name}: {prop.Value}";
							propItem.Tag = prop.Value;
							itemNode.Items.Add(propItem);
						}
					}
				}
			}
		}

		private void UpdateChildren(JToken childNode, TreeViewItem parentItem)
		{
			if (childNode is JObject objChild)
			{
				// Handle nested objects
				foreach (var prop in objChild.Properties())
				{
					TreeViewItem propItem = new TreeViewItem();
					propItem.Header = $"{prop.Name}: {prop.Value}";
					propItem.Tag = prop.Value;
		
					UpdateChildren(prop.Value, propItem);
		
					parentItem.Items.Add(propItem);
				}
			}
			else if (childNode is JArray arrChild)
			{
				// Handle nested arrays
				for (int i = 0; i < arrChild.Count; i++)
				{
					TreeViewItem arrItem = new TreeViewItem();
					arrItem.Header = $"[{i}]: {arrChild[i].ToString(Formatting.None)}";
					arrItem.Tag = arrChild[i];
		
					UpdateChildren(arrChild[i], arrItem);
		
					parentItem.Items.Add(arrItem);
				}
			}
			else
			{
				TreeViewItem leafNode = new TreeViewItem();
				leafNode.Header = childNode.ToString(Formatting.None);
				leafNode.Tag = childNode;
				parentItem.Items.Add(leafNode);
			}
		}
	}

	public class SaveToJSON
	{


		string SaveData = JsonConvert.SerializeObject("");
	}
}