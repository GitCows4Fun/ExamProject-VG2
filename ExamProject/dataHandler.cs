using System;
using System.Collections.Generic;
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
        public static string upgrades { get; set; } = @"../../../data/upgrades.json";
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

        private DataGrid _dataGrid;
        private TreeViewItem _currentSelectedItem;
        private Stack<object> _navigationHistory = new Stack<object>();

        public Stack<object> NavigationHistory { get => _navigationHistory; }
        public TreeViewItem CurrentSelectedItem { get => _currentSelectedItem; set => _currentSelectedItem = value; }

        public void InitializeViews(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
        }

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
                _dataGrid.ItemsSource = JsonData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateDataGrid(JToken currentToken)
        {
            if (currentToken is JArray array)
            {
                JsonData = JsonParser.ParseJArray(array);
            }
            else if (currentToken is JObject obj)
            {
                JsonData = new List<Dictionary<string, object>>()
                {
                    JsonParser.ParseJsonObject(obj)
                };
            }
            else
            {
                JsonData = null;
            }
        }

        public void UpdateTreeView(JToken currentNode, TreeViewItem parent = null)
        {
            TreeViewItem newTreeViewItem = new TreeViewItem();
            newTreeViewItem.Header = currentNode.ToString(Formatting.None);
            newTreeViewItem.Tag = currentNode;

            if (currentNode is JObject jsonObject)
            {
                foreach (var prop in jsonObject.Properties())
                {
                    UpdateTreeView(prop.Value, newTreeViewItem);
                }
            }
            else if (currentNode is JArray jsonArray)
            {
                UpdateTreeView(jsonArray[0], newTreeViewItem);
            }

            parent?.Items.Add(newTreeViewItem);
        }
    }
}