using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task CreateWallpaperAsync(Wallpaper wallpaper)
        {
            using (SqliteConnection cnn = GetConnection())
            {
                await cnn.OpenAsync();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = cnn;

                insertCommand.CommandText = "INSERT INTO WallpaperHistory (Id, Title, Author, IsSaved, AuthorProfileUrl, ImageUrl, SourceUrl, Provider) VALUES (@Id, @Title, @Author, @IsSaved, @AuthorProfileUrl, @ImageUrl, @SourceUrl, @Provider);";
                insertCommand.Parameters.AddWithValue("@Id", wallpaper.Id);
                insertCommand.Parameters.AddWithValue("@Title", wallpaper.Title);
                insertCommand.Parameters.AddWithValue("@Author", wallpaper.Author);
                insertCommand.Parameters.AddWithValue("@IsSaved", wallpaper.IsSaved ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@AuthorProfileUrl", wallpaper.AuthorProfileUrl);
                insertCommand.Parameters.AddWithValue("@ImageUrl", wallpaper.ImageUrl);
                insertCommand.Parameters.AddWithValue("@SourceUrl", wallpaper.SourceUrl);
                insertCommand.Parameters.AddWithValue("@Provider", wallpaper.Provider);

                await insertCommand.ExecuteReaderAsync();
            }
        }

        public async Task<Wallpaper> ReadWallpaperAsync(string id)
        {
            Wallpaper queryResult = null;
            
            using (SqliteConnection cnn = GetConnection())
            {
                await cnn.OpenAsync();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = cnn;
                selectCommand.CommandText = $"SELECT * from WallpaperHistory WHERE Id='{id}'";

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var wallpaper = new Wallpaper
                    {
                        Id = query.GetString(0),
                        Title = query.GetString(1),
                        Author = query.GetString(2),
                        IsSaved = query.GetInt32(3) == 1,
                        AuthorProfileUrl = query.GetString(4),
                        ImageUrl = query.GetString(5),
                        SourceUrl = query.GetString(6),
                        Provider = query.GetString(7)
                    };

                    queryResult = wallpaper;
                }
            }

            return queryResult;
        }

        public async Task<List<Wallpaper>> ReadAllWallpapersAsync()
        {
            var entries = new List<Wallpaper>();

            using (SqliteConnection cnn = GetConnection())
            {
                await cnn.OpenAsync();

                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = cnn;
                selectCommand.CommandText = "SELECT * from WallpaperHistory";

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var wallpaper = new Wallpaper
                    {
                        Id = query.GetString(0),
                        Title = query.GetString(1),
                        Author = query.GetString(2),
                        IsSaved = query.GetInt32(3) == 1,
                        AuthorProfileUrl = query.GetString(4),
                        ImageUrl = query.GetString(5),
                        SourceUrl = query.GetString(6),
                        Provider = query.GetString(7)
                    };

                    entries.Add(wallpaper);
                }
            }

            return entries;
        }

        public async Task<bool> UpdateWallpaperAsync(Wallpaper wallpaper)
        {
            var wallpaperToUpdate = await ReadWallpaperAsync(wallpaper.Id);

            if (wallpaperToUpdate is null)
            {
                return false;
            }

            using (SqliteConnection cnn = GetConnection())
            {
                await cnn.OpenAsync();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = cnn;

                insertCommand.CommandText = $"UPDATE WallpaperHistory SET (Id, Title, Author, IsSaved, AuthorProfileUrl, ImageUrl, SourceUrl, Provider)=(@Id, @Title, @Author, @IsSaved, @AuthorProfileUrl, @ImageUrl, @SourceUrl, @Provider) WHERE Id='{wallpaper.Id}'";
                insertCommand.Parameters.AddWithValue("@Id", wallpaper.Id);
                insertCommand.Parameters.AddWithValue("@Title", wallpaper.Title);
                insertCommand.Parameters.AddWithValue("@Author", wallpaper.Author);
                insertCommand.Parameters.AddWithValue("@IsSaved", wallpaper.IsSaved ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@AuthorProfileUrl", wallpaper.AuthorProfileUrl);
                insertCommand.Parameters.AddWithValue("@ImageUrl", wallpaper.ImageUrl);
                insertCommand.Parameters.AddWithValue("@SourceUrl", wallpaper.SourceUrl);
                insertCommand.Parameters.AddWithValue("@Provider", wallpaper.Provider);

                await insertCommand.ExecuteReaderAsync();
            }

            return true;
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
                        + "IsSaved BOOLEAN NOT NULL CHECK (IsSaved IN (0,1)), "
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
