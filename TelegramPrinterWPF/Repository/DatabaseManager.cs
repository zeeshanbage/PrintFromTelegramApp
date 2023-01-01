using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramPrinterWPF.Models;

namespace TelegramPrinterWPF.Repository
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {

        }

        public void FirstTimeSetup()
        {
            File.WriteAllBytes("TelegramPrinter.db", new byte[0]);
            using var connection = GetConnection();

            connection.Open();

            var command = connection.CreateCommand();
            //command.CommandText = "DROP TABLE DownloadedFileInfo";
            command.CommandText = " CREATE TABLE  DownloadedFileInfo (FileId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT,FileInfo_Json TEXT)";
            command.ExecuteNonQuery();
        }

    

        public void saveDocFile(DocFile file)
        {
            using var connection = GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO DownloadedFileInfo (name,fileInfo_json) VALUES(@Name,@FileInfo_Json)";
            command.Parameters.AddWithValue("@Name", file.Name);
            command.Parameters.AddWithValue("@FileInfo_Json", JsonSerializer.Serialize(file));
            command.ExecuteNonQuery();

        }

        public List<DocFile> GetDocFiles()
        {
            using var connection = GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"Select Name,FileInfo_Json FROM DownloadedFileInfo";
            var reader = command.ExecuteReader();

            List<DocFile> FileList = new List<DocFile>();
            while(reader.Read())
            {
                var Docfile = reader.GetString(1);
                FileList.Add(JsonSerializer.Deserialize<DocFile>(Docfile));
            }
            return FileList;
        }

        public DocFile GetDoc(string name)
        {
            using var connection = GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"Select Name,FileInfo_Json FROM DownloadedFileInfo WHERE Name=@name";
            command.Parameters.AddWithValue("@Name", name);
            var reader = command.ExecuteScalar() as string;
            DocFile doc = JsonSerializer.Deserialize<DocFile>(reader);
            return doc;

        }
        private static SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection("Data source=TelegramPrinter.db");
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            return connection;
        }
    }
}
