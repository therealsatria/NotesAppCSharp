using CsvHelper;
using Microsoft.Data.Sqlite;
using NotesAppCSharp.Services;
using System.Globalization;

namespace NotesAppCSharp.Functions;

public static class ImportFromCsv
{
    public static void Execute(DatabaseService dbService)
    {
        Console.Write("Masukkan path file CSV untuk diimpor (default: 'notes_import.csv'): ");
        string? path = Console.ReadLine()?.Trim();
        path = string.IsNullOrEmpty(path) ? "notes_import.csv" : path;

        if (!File.Exists(path))
        {
            Console.WriteLine($"File '{path}' tidak ditemukan!");
            return;
        }

        var records = new List<dynamic>();
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<dynamic>().ToList();
        }

        using var connection = dbService.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var clearCommand = connection.CreateCommand();
        clearCommand.CommandText = "DELETE FROM notes";
        clearCommand.ExecuteNonQuery();

        foreach (var record in records)
        {
            int id = int.TryParse(record.Id?.ToString(), out int i) ? i : 0;
            string note = record.Text?.ToString() ?? "";
            string priority = record.Priority?.ToString() ?? "Sedang";
            string createdAt = DateTime.TryParse(record.CreatedAt?.ToString(), out DateTime ca) ? ca.ToString("O") : DateTime.UtcNow.ToString("O");
            string? modifiedAt = string.IsNullOrEmpty(record.ModifiedAt?.ToString()) ? null : DateTime.Parse(record.ModifiedAt.ToString()).ToString("O");

            using var command = connection.CreateCommand();
            command.CommandText = id == 0
                ? "INSERT INTO notes (note, priority, createdAt, modifiedAt) VALUES (@note, @priority, @createdAt, @modifiedAt)"
                : "INSERT INTO notes (id, note, priority, createdAt, modifiedAt) VALUES (@id, @note, @priority, @createdAt, @modifiedAt)";
            if (id != 0) command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@note", dbService.Encrypt(note)); // Gunakan metode publik
            command.Parameters.AddWithValue("@priority", dbService.Encrypt(priority)); // Gunakan metode publik
            command.Parameters.AddWithValue("@createdAt", createdAt);
            command.Parameters.AddWithValue("@modifiedAt", modifiedAt ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        transaction.Commit();
        Console.WriteLine($"Data berhasil diimpor dari '{path}'");
    }
}