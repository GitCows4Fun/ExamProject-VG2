using System.Text.Json;			// Guess what? We need this for JSON
using System.Diagnostics;		// For Debugging, dipshit. 
using MongoDB.Bson;
using MongoDB.Driver;           // Both needed for MongoDB 
using System.Text.RegularExpressions;  // Needed for Regex
using ExamProject;
using System.Collections.Generic;

namespace DataImport
{
	public class DataRetrieve
	{
		public string[] getDataFromDB(string db_ip, string db_port)
		{
			return new string[] { "data1", "data2" };
		}
	}
	

	private class Record 
	{


		public string? Id { get; set; }  // Nullable to avoid warnings -- although this shouldnt be null at any point
		public string? Name { get; set; }
		public string? Lore { get; set; }
		public string? Alignments { get; set; }
		public double? Chance { get; set; }  // Nullable to prevent warnings
		public string? Class { get; set; }
	}
		private async Task LoadFromMongoDB()
		{
			try
			{
				if (Regex.IsMatch(txtDBPath.Text, @"^(?:localhost|\d{1,3}(?:\.\d{1,3}){3})(?::\d+)?$") && !string.IsNullOrEmpty(txtDBPath.Text))
				{
					Debug.WriteLine("mongodb path is valid: match = " + Regex.Match(txtDBPath.Text, @"^(?:[\d\.]+|localhost):\d+$"));
					var client = new MongoClient("mongodb://" + txtDBPath.Text);
					if (!string.IsNullOrEmpty(txtDBName.Text) && txtDBName.Text != "Database" &&
						!string.IsNullOrEmpty(txtDBColl.Text) && txtDBColl.Text != "Collection")
					{
						var database = client.GetDatabase(txtDBName.Text);
						mongoCollection = database.GetCollection<BsonDocument>(txtDBColl.Text);
					}
					else
					{
						return;
					}
				}
				else if (string.IsNullOrEmpty(txtDBPath.Text))
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
				if (string.IsNullOrEmpty(txtJSONPath.Text))
				{
					return;
				}
				else
				{
					jsonFilePath = txtJSONPath.Text.Replace(@"\", @"\\");
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
