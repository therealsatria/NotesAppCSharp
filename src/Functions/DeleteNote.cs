using Microsoft.Data.Sqlite;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class DeleteNote
{
    public static void Execute(DatabaseService dbService)
    {
        Console.Write("Masukkan ID catatan yang akan dihapus: ");
        string? idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("ID tidak valid!");
            return;
        }

        using var connection = dbService.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM notes WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
            Console.WriteLine($"Catatan dengan ID {id} berhasil dihapus!");
        else
            Console.WriteLine($"Catatan dengan ID {id} tidak ditemukan!");
    }
}