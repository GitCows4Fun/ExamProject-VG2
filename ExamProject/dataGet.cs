using System.Text.Json;			// Guess what? We need this for JSON
using System.Diagnostics;		// For Debugging, dipshit. 
using MongoDB.Bson;
using MongoDB.Driver;           // Both needed for MongoDB 
using System.Text.RegularExpressions;  // Needed for Regex
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.IO; 
using ExamProject;

namespace DataImport
{
	public class DataRetrieve
	{
		public string[] getDataFromDB(string db_ip, string db_port)
		{
			return new string[] { "data1", "data2" };
		}
	}
	

	class Record
	{
		public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();
		private string? jsonFilePath;
		private IMongoCollection<BsonDocument>? mongoCollection; // MongoDB collection variable


		public string? Id { get; set; }  // Nullable to avoid warnings -- although this shouldnt be null at any point
		public string? Name { get; set; }
		public string? Lore { get; set; }
		public string? Alignments { get; set; }
		public double? Chance { get; set; }  // Nullable to prevent warnings
		public string? Class { get; set; }
#pragma warning disable CS1519
	} 
#pragma warning restore CS1519 

		private async Task LoadFromMongoDB()
		{
			try
			{
				if (Regex.IsMatch("", @"^(?:localhost|\d{1,3}(?:\.\d{1,3}){3})(?::\d+)?$") && !string.IsNullOrEmpty(""))
				{
					Debug.WriteLine("mongodb path is valid: match = " + Regex.Match("", @"^(?:[\d\.]+|localhost):\d+$"));
					var client = new MongoClient("mongodb://" + "");
					if (!string.IsNullOrEmpty("") && "" != "Database" &&
						!string.IsNullOrEmpty("") && "" != "Collection")
					{
						var database = client.GetDatabase("");
						mongoCollection = database.GetCollection<BsonDocument>("");
					}
					else
					{
						return;
					}
				}
				else if (string.IsNullOrEmpty(""))
				{
					var client = new MongoClient("mongodb://localhost:27017");
					var database = client.GetDatabase("qb");
					mongoCollection = database.GetCollection<BsonDocument>("deity");
				}

				if (mongoCollection == null)
				{
					return;
				}

				var records = await mongoCollection.Find(new BsonDocument()).ToListAsync();
				Records.Clear();

				foreach (var bsonDoc in records)
				{
					var newRecord = new Record
					{
						Id = bsonDoc.Contains("_id") ? bsonDoc["_id"].ToString() : "Unknown",
						Name = bsonDoc.Contains("name") ? bsonDoc["name"].ToString() : "Unknown",
						Lore = bsonDoc.Contains("lore") ? bsonDoc["lore"].ToString() : "Unknown",
						Alignments = bsonDoc.Contains("alignments") ? bsonDoc["alignments"].ToString() : "None",
						Chance = bsonDoc.Contains("chance") ? (double?)bsonDoc["chance"].ToDouble() : null,
						Class = bsonDoc.Contains("class") ? bsonDoc["class"].ToString() : "Unknown"
					};

					Records.Add(newRecord);
				}
			}
			catch (System.Net.Sockets.SocketException ex)
			{
				return;
			}
			catch (MongoConnectionException ex)
			{
				return;
			}
			catch (Exception ex)
			{
				return;
			}
		}

		private void LoadFromJson()
		{
			try
			{
				if (string.IsNullOrEmpty(""))
				{
					return;
				}
				else
				{
					jsonFilePath = "".Replace(@"\", @"\\");
				}


				if (File.Exists(jsonFilePath))  // Check if the JSON file exists
				{
					string json = File.ReadAllText(jsonFilePath);
					var records = JsonSerializer.Deserialize<List<Record>>(json);

					Records.Clear();
					foreach (var record in records)
					{
						Records.Add(record);
					}
				}
				else
				{
					return;
				}
			}
			catch (FileNotFoundException ex)
			{
				return;
			}
			catch (Exception ex)
			{
				return;
			}
		}
	}
}
