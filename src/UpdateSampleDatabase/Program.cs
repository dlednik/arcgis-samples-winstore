using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSampleDatabase
{
	class Program
	{
		static void Main(string[] args)
		{
			var sampleFolder = args[0];
			var database = args[1];
			if (File.Exists(database))
				File.Delete(database);
			SQLiteConnection conn = new SQLiteConnection("Data source=" + database);
			conn.Open();
			Console.WriteLine("Creating table...");
			string sql = "CREATE TABLE 'SampleData' (Id INTEGER PRIMARY KEY, 'Group' TEXT NOT NULL, Name TEXT NOT NULL, CsCode TEXT, VbCode TEXT, Xaml TEXT, Description TEXT);";
			SQLiteCommand cmd = new SQLiteCommand(sql, conn);
			int result = cmd.ExecuteNonQuery();
			sql = "INSERT INTO 'SampleData' VALUES(null, @group, @name, @cscode, @vbcode, @xaml, @description)";
			foreach (var folder in new DirectoryInfo(sampleFolder).GetDirectories())
			{
				string group = folder.Name;
				Console.WriteLine("Processing group : " + group);

				foreach (var file in folder.EnumerateFiles("*.xaml"))
				{
					string name = file.Name.Substring(0, file.Name.IndexOf("."));
					Console.WriteLine("   Creating " + name);
					string xaml = file.OpenText().ReadToEnd();
					string cscode = null;
					string vbcode = null;
					string description = null;
					if (File.Exists(file.FullName + ".cs"))
					{
						cscode = new FileInfo(file.FullName + ".cs").OpenText().ReadToEnd();
					}
					if (File.Exists(file.FullName + ".txt"))
					{
						description = new FileInfo(file.FullName + ".txt").OpenText().ReadToEnd();
					}
					
					cmd = new SQLiteCommand(sql, conn);
					cmd.Parameters.Add(new SQLiteParameter("Group", group));
					cmd.Parameters.Add(new SQLiteParameter("Name", name));
					cmd.Parameters.Add(new SQLiteParameter("CsCode", cscode));
					cmd.Parameters.Add(new SQLiteParameter("VbCode", vbcode));
					cmd.Parameters.Add(new SQLiteParameter("Xaml", xaml));
					cmd.Parameters.Add(new SQLiteParameter("Description", description));
					result = cmd.ExecuteNonQuery();
					if (result == 0)
						throw new Exception();
				}
			}
			sql = "CREATE INDEX sampledata_group ON SampleData ('Group')";
			cmd = new SQLiteCommand(sql, conn);
			cmd.ExecuteNonQuery();
			conn.Close();
		}
	}
}
