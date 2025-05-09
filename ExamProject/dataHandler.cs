using System;
using System.IO;
using ExamProject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExamProject 
{
	public class JsonParser
	{
		public static JArray ReadJsonFromFile(string filePath)
		{
			try
			{
				// Read the JSON content from the specified file
				string json = File.ReadAllText(filePath);

				// Parse the JSON into a dynamic JArray
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

		//public static void ProcessJsonData()
		//{
		//	string filePath = @"C:\path\to\your\file.json"; // JSON file path

		//	try
		//	{
		//		// Call the ReadJsonFromFile method to get the parsed JSON data
		//		JArray jsonData = JsonParser.ReadJsonFromFile(filePath);

		//		// Iterate over each item in the JSON array
		//		foreach (JObject item in jsonData)
		//		{
		//			// Access properties dynamically and write them to constants :3 (TBD) 
		//			Console.WriteLine("ID: " + item["id"].ToString());
		//			Console.WriteLine("Name: " + item["name"].ToString());
		//			Console.WriteLine("Lore: " + item["lore"].ToString());

		//			// Access nested structures if available
		//			if (item["unlocks"] != null)
		//			{
		//				JArray unlocks = (JArray)item["unlocks"];
		//				Console.WriteLine("Unlocks:");
		//				foreach (JObject unlock in unlocks)
		//				{
		//					Console.WriteLine("- ID: " + unlock["id"].ToString());
		//					Console.WriteLine("- Description: " + unlock["desc"].ToString());
		//				}
		//			}
		//			Console.WriteLine("\n");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine("An error occurred: " + ex.Message);
		//	}
		//}
	}
}
