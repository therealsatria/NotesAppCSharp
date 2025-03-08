using Microsoft.Data.Sqlite;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class EditNote
{
    public static void Execute(DatabaseService dbService, int? providedId)
    {
        int id = providedId ?? GetIdFromUser();
        Console.Write("\nMasukkan catatan baru (max 255 char, kosongkan untuk tidak mengubah): ");
        string? note = Console.ReadLine()?.Trim();

        Console.Write("Masukkan prioritas baru (1: Tinggi, 2: Sedang, 3: Rendah, 0: Tidak ubah): ");
        string? prioChoice = Console.ReadLine()?.Trim();
        int prioValue = string.IsNullOrEmpty(prioChoice) ? 0 : int.TryParse(prioChoice, out int value) ? value : 0;

        string? priority = prioValue switch
        {
            1 => "Tinggi",
            2 => "Sedang",
            3 => "Rendah",
            0 => null,
            _ => null
        };

        string timestamp = DateTime.UtcNow.ToString("O");

        using var connection = dbService.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        if (!string.IsNullOrEmpty(note) && priority != null)
        {
            command.CommandText = "UPDATE notes SET note = @note, priority = @priority, modifiedAt = @modifiedAt WHERE id = @id";
            command.Parameters.AddWithValue("@note", dbService.Encrypt(note)); // Gunakan metode publik
            command.Parameters.AddWithValue("@priority", dbService.Encrypt(priority)); // Gunakan metode publik
            command.Parameters.AddWithValue("@modifiedAt", timestamp);
            command.Parameters.AddWithValue("@id", id);
        }
        else if (!string.IsNullOrEmpty(note))
        {
            command.CommandText = "UPDATE notes SET note = @note, modifiedAt = @modifiedAt WHERE id = @id";
            command.Parameters.AddWithValue("@note", dbService.Encrypt(note)); // Gunakan metode publik
            command.Parameters.AddWithValue("@modifiedAt", timestamp);
            command.Parameters.AddWithValue("@id", id);
        }
        else if (priority != null)
        {
            command.CommandText = "UPDATE notes SET priority = @priority, modifiedAt = @modifiedAt WHERE id = @id";
            command.Parameters.AddWithValue("@priority", dbService.Encrypt(priority)); // Gunakan metode publik
            command.Parameters.AddWithValue("@modifiedAt", timestamp);
            command.Parameters.AddWithValue("@id", id);
        }
        else
        {
            Console.WriteLine("Tidak ada perubahan yang dibuat.");
            return;
        }

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
            Console.WriteLine($"Catatan dengan ID {id} berhasil diperbarui!");
        else
            Console.WriteLine($"Catatan dengan ID {id} tidak ditemukan!");
    }

    private static int GetIdFromUser()
    {
        Console.Write("Masukkan ID catatan yang akan diedit: ");
        string? idInput = Console.ReadLine();
        return int.TryParse(idInput, out int id) ? id : 0;
    }
}