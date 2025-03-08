using Microsoft.Data.Sqlite;
using NotesAppCSharp.Models;

namespace NotesAppCSharp.Services;

public class DatabaseService
{
    private readonly string _connectionString;
    private readonly EncryptionService _encryptionService;

    public DatabaseService(string dbPath, EncryptionService encryptionService)
    {
        _connectionString = $"Data Source={dbPath}";
        _encryptionService = encryptionService;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS notes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                note BLOB NOT NULL,
                priority BLOB NOT NULL,
                createdAt TEXT NOT NULL,
                modifiedAt TEXT
            )";
        command.ExecuteNonQuery();
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public byte[] Encrypt(string data)
    {
        return _encryptionService.Encrypt(data);
    }

    public string Decrypt(byte[] encryptedData)
    {
        return _encryptionService.Decrypt(encryptedData);
    }

    public List<Note> GetNotes(string query, params SqliteParameter[] parameters)
    {
        var notes = new List<Note>();
        using var connection = GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddRange(parameters);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            notes.Add(new Note
            {
                Id = reader.GetInt32(0),
                Text = Decrypt((byte[])reader.GetValue(1)), // Gunakan metode publik
                Priority = Decrypt((byte[])reader.GetValue(2)), // Gunakan metode publik
                CreatedAt = DateTime.Parse(reader.GetString(3)),
                ModifiedAt = reader.IsDBNull(4) ? null : DateTime.Parse(reader.GetString(4))
            });
        }

        return notes;
    }
}