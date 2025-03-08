using Microsoft.Data.Sqlite;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class AddNote
{
    public static void Execute(DatabaseService dbService)
    {
        Console.Write("Masukkan catatan (max 255 char): ");
        string? note = Console.ReadLine()?.Trim();

        Console.Write("Masukkan prioritas (1: Tinggi, 2: Sedang, 3: Rendah, kosongkan untuk default Sedang): ");
        string? prioChoice = Console.ReadLine()?.Trim();
        int prioValue = string.IsNullOrEmpty(prioChoice) ? 2 : int.TryParse(prioChoice, out int value) ? value : 2;

        string priority = prioValue switch
        {
            1 => "Tinggi",
            2 => "Sedang",
            3 => "Rendah",
            _ => "Sedang" // Default untuk input tidak valid
        };

        if (string.IsNullOrEmpty(note))
        {
            Console.WriteLine("Catatan tidak boleh kosong!");
            return;
        }

        string timestamp = DateTime.UtcNow.ToString("O"); // Format RFC 3339

        using var connection = dbService.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO notes (note, priority, createdAt, modifiedAt) VALUES (@note, @priority, @createdAt, @modifiedAt)";
        command.Parameters.AddWithValue("@note", dbService.Encrypt(note)); // Gunakan metode publik
        command.Parameters.AddWithValue("@priority", dbService.Encrypt(priority)); // Gunakan metode publik
        command.Parameters.AddWithValue("@createdAt", timestamp);
        command.Parameters.AddWithValue("@modifiedAt", timestamp);

        command.ExecuteNonQuery();
        Console.WriteLine("Catatan berhasil ditambahkan!");
    }
}