using System.Text.Json;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Threading.Tasks;
using ExamProject;

namespace DataImport
{

	public class Upgrades
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public int Cost { get; set; }
		public int Currency { get; set; }
		public double CostScale { get; set; }
		public double EffectScale { get; set; }
		public int MaxLevel { get; set; }
		public double EffectLow { get; set; }
		public double EffectHigh { get; set; }
		public double EffectRe { get; set; }
		public double EffectPre { get; set; }
		public double EffectAsc { get; set; }
		public double EffectSac { get; set; }
		public double EffectDe { get; set; }
	}

	public class DataHandler
	{
#pragma warning disable CS1998
#pragma warning disable CS1822
		// This is what will initially load the base values to be used from variousjson files in the appropriate folder 
		public async Task InitializeBaseValues()
		{

		}

		private string? jsonFilePath;
		private IMongoCollection<BsonDocument>? mongoCollection;
		public ObservableCollection<Upgrades> Upgrades { get; set; } = new ObservableCollection<Upgrades>();

		public DataHandler(string jsonFilePath)
		{
			this.jsonFilePath = jsonFilePath;
		}

		private async Task LoadFromMongoDB(string db_ip, string db_port, string databaseName, string collectionName, string documentName)
		{
			try
			{
				if (Regex.IsMatch(db_ip, @"^(?:localhost|\d{1,3}(?:\.\d{1,3}){3})(?::\d+)?$") && !string.IsNullOrEmpty(db_port))
				{
					Debug.WriteLine("mongodb path is valid: match = " + Regex.Match(db_ip + ":" + db_port, @"^(?:[\d\.]+|localhost):\d+$"));
					var client = new MongoClient("mongodb://" + db_ip + ":" + db_port);
					if (!string.IsNullOrEmpty(databaseName) && databaseName != "Database" &&
						!string.IsNullOrEmpty(collectionName) && collectionName != "Collection")
					{
						var database = client.GetDatabase(databaseName);
						mongoCollection = database.GetCollection<BsonDocument>(collectionName);
					}
					else
					{
						return;
					}
				}
				else if (string.IsNullOrEmpty(db_ip) && string.IsNullOrEmpty(db_port))
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
				Upgrades.Clear();

				foreach (var bsonDoc in records)
				{
					var newUpgrade = new Upgrades
					{
						Id = bsonDoc.Contains("_id") ? bsonDoc["_id"].AsInt32 : 0,
						Name = bsonDoc.Contains("name") ? bsonDoc["name"].AsString : "Unknown",
						Description = bsonDoc.Contains("description") ? bsonDoc["description"].AsString : "Unknown",
						Cost = bsonDoc.Contains("cost") ? bsonDoc["cost"].AsInt32 : 0,
						Currency = bsonDoc.Contains("currency") ? bsonDoc["currency"].AsInt32 : 0,
						CostScale = bsonDoc.Contains("costscale") ? bsonDoc["costscale"].AsInt32 : 0,
						EffectScale = bsonDoc.Contains("effectscale") ? bsonDoc["effectscale"].AsDouble : 0,
						MaxLevel = bsonDoc.Contains("maxlevel") ? bsonDoc["maxlevel"].AsInt32 : 0,
						EffectLow = bsonDoc.Contains("effectlow") ? bsonDoc["effectlow"].AsDouble : 0,
						EffectHigh = bsonDoc.Contains("effecthigh") ? bsonDoc["effecthigh"].AsDouble : 0,
						EffectRe = bsonDoc.Contains("effectre") ? bsonDoc["effectre"].AsDouble : 0,
						EffectPre = bsonDoc.Contains("effectpre") ? bsonDoc["effectpre"].AsDouble : 0,
						EffectAsc = bsonDoc.Contains("effectasc") ? bsonDoc["effectasc"].AsDouble : 0,
						EffectSac = bsonDoc.Contains("effectsac") ? bsonDoc["effectsac"].AsDouble : 0,
						EffectDe = bsonDoc.Contains("effectde") ? bsonDoc["effectde"].AsDouble : 0
					};

					Upgrades.Add(newUpgrade);
				}
			}
			catch (System.Net.Sockets.SocketException ex)
			{
				Debug.WriteLine("SocketException: " + ex.Message);
			}
			catch (MongoConnectionException ex)
			{
				Debug.WriteLine("MongoConnectionException: " + ex.Message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Exception: " + ex.Message);
			}
		}

		public void LoadFromJson()
		{
			try
			{
				if (string.IsNullOrEmpty(jsonFilePath))
				{
					Debug.WriteLine("JSON file path is not provided.");
					return;
				}
				else
				{
					jsonFilePath = jsonFilePath.Replace(@"\", @"\\");
				}

				if (File.Exists(jsonFilePath))  // Check if the JSON file exists
				{
					string json = File.ReadAllText(jsonFilePath);
					var upgrades = JsonSerializer.Deserialize<List<Upgrades>>(json);

					Upgrades.Clear();
					foreach (var upgrade in upgrades)
					{
						Upgrades.Add(upgrade);
					}
				}
				else
				{
					Debug.WriteLine("JSON file does not exist: " + jsonFilePath);
				}
			}
			catch (FileNotFoundException ex)
			{
				Debug.WriteLine("FileNotFoundException: " + ex.Message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Exception: " + ex.Message);
			}
		}
	}
}