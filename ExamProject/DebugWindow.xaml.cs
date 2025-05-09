using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExamProject
{
	public partial class DebugWindow : Window
	{

		private class JSON_FILE_PATH
		{
			public static string upgrades { get; set; } = @"../../upgrades.json";
		};

		public DebugWindow()
		{
			InitializeComponent();
			LoadJsonData();
		}

		private void LoadJsonData()
		{
			try
			{
				string jsonContent = File.ReadAllText(JSON_FILE_PATH.upgrades);
				dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);

				if (jsonData is JArray jsonArray)
				{
					List<Dictionary<string, object>> dataToList = ConvertJArrayToList(jsonArray);
					DataGrid.ItemsSource = dataToList;
				}
				else if (jsonData is JObject jsonObject)
				{
					// Handle single object case if needed
					List<Dictionary<string, object>> singleItem = new List<Dictionary<string, object>>();
					singleItem.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(jsonObject)));
					DataGrid.ItemsSource = singleItem;
				}
				else
				{
					MessageBox.Show("The JSON content is neither an array nor an object.");
				}
			}
			catch (FileNotFoundException)
			{
				MessageBox.Show("The JSON file was not found."); 
			}
			catch (JsonException ex)
			{
				MessageBox.Show($"Error parsing JSON: {ex.Message}"); 
            }
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred: {ex.Message}"); 
            }
		}

		private List<Dictionary<string, object>> ConvertJArrayToList(JArray jArray)
		{
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (JObject obj in jArray)
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				foreach (var prop in obj.Properties())
				{
					dict[prop.Name] = prop.Value.ToObject<object>();
				}
				list.Add(dict);
				// Handle nested structures if necessary
			}
			return list;
		}
	}
}