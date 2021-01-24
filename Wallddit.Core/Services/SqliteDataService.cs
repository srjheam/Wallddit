using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

using Wallddit.Core.Models;

namespace Wallddit.Core.Services
{
    public class SqliteDataService
    {
        public string ConnectionString { get; private set; }

        public SqliteDataService(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                throw new ArgumentException($"'{nameof(dbPath)}' cannot be null or whitespace", nameof(dbPath));
            }

            ConnectionString = $"Data Source={dbPath};";

            InitializeDatabase();
        }

        public SqliteConnection GetConnection() => new SqliteConnection(ConnectionString);

        public void AddWallpaper(Wallpaper wallpaper)
        {
            using (SqliteConnection cnn = GetConnection())
            {
                cnn.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = cnn;

                insertCommand.CommandText = "INSERT INTO WallpaperHistory (Id, Title, Author, AuthorProfileUrl, ImageUrl, SourceUrl, Provider) VALUES (@Id, @Title, @Author, @AuthorProfileUrl, @ImageUrl, @SourceUrl, @Provider);";
                insertCommand.Parameters.AddWithValue("@Id", wallpaper.Id);
                insertCommand.Parameters.AddWithValue("@Title", wallpaper.Title);
                insertCommand.Parameters.AddWithValue("@Author", wallpaper.Author);
                insertCommand.Parameters.AddWithValue("@AuthorProfileUrl", wallpaper.AuthorProfileUrl);
                insertCommand.Parameters.AddWithValue("@ImageUrl", wallpaper.ImageUrl);
                insertCommand.Parameters.AddWithValue("@SourceUrl", wallpaper.SourceUrl);
                insertCommand.Parameters.AddWithValue("@Provider", wallpaper.Provider);

                insertCommand.ExecuteReader();
            }
        }

        public List<Wallpaper> GetWallpapers()
        {
            var entries = new List<Wallpaper>();

            using (SqliteConnection cnn = GetConnection())
            {
                cnn.Open();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = cnn;
                selectCommand.CommandText = "SELECT * from WallpaperHistory";

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    var wallpaper = new Wallpaper
                    {
                        Id = query.GetString(0),
                        Title = query.GetString(1),
                        Author = query.GetString(2),
                        AuthorProfileUrl = query.GetString(3),
                        ImageUrl = query.GetString(4),
                        SourceUrl = query.GetString(5),
                        Provider = query.GetString(6)
                    };

                    entries.Add(wallpaper);
                }
            }

            return entries;
        }

        private void InitializeDatabase()
        {
            using (var cnn = GetConnection())
            {
                cnn.Open();

                using (var command = cnn.CreateCommand())
                {
                    const string TABLE_COMMAND = "CREATE TABLE IF NOT EXISTS WallpaperHistory ("
                        + "Id TEXT NOT NULL UNIQUE, "
                        + "Title TEXT NOT NULL, "
                        + "Author TEXT NOT NULL, "
                        + "AuthorProfileUrl TEXT NOT NULL, "
                        + "ImageUrl TEXT NOT NULL, "
                        + "SourceUrl TEXT NOT NULL, "
                        + "Provider TEXT NOT NULL, "
                        + "PRIMARY KEY(Id));";

                    command.CommandText = TABLE_COMMAND;

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
