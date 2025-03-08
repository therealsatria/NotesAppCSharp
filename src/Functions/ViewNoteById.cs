using Microsoft.Data.Sqlite;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class ViewNoteById
{
    public static void Execute(DatabaseService dbService)
    {
        Console.Write("Masukkan ID catatan yang ingin dilihat: ");
        string? idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("ID tidak valid!");
            return;
        }

        string query = "SELECT id, note, priority, createdAt, modifiedAt FROM notes WHERE id = @id";
        var notes = dbService.GetNotes(query, new SqliteParameter("@id", id));

        if (notes.Count > 0)
        {
            var note = notes[0];
            Console.WriteLine("\nDetail Catatan:");
            Console.WriteLine($"ID         : {note.Id}");
            Console.WriteLine($"Catatan    : {note.Text}");
            Console.WriteLine($"Prioritas  : {note.Priority}");
            Console.WriteLine($"Dibuat     : {note.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            if (note.ModifiedAt.HasValue)
                Console.WriteLine($"Diperbarui : {note.ModifiedAt.Value:yyyy-MM-dd HH:mm:ss}");

            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Edit Catatan");
            Console.WriteLine("2. Hapus Catatan");
            Console.WriteLine("3. Ganti Prioritas");
            Console.WriteLine("4. Kembali ke Menu Utama");
            Console.Write("Pilih opsi (1-4): ");

            string? choice = Console.ReadLine();
            if (int.TryParse(choice ?? "", out int option)) // Sudah diperbaiki dengan ?? ""
            {
                switch (option)
                {
                    case 1: EditNote.Execute(dbService, note.Id); break;
                    case 2: DeleteNote.Execute(dbService); break;
                    case 3: ChangePriority(dbService, note.Id); break;
                    case 4: Console.WriteLine("Kembali ke menu utama."); break;
                    default: Console.WriteLine("Pilihan tidak valid!"); break;
                }
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid!");
            }
        }
        else
        {
            Console.WriteLine($"Catatan dengan ID {id} tidak ditemukan!");
        }
    }

    private static void ChangePriority(DatabaseService dbService, int id)
    {
        Console.Write("Masukkan prioritas baru (1: Tinggi, 2: Sedang, 3: Rendah): ");
        string? prioChoice = Console.ReadLine();
        int prioValue = int.TryParse(prioChoice, out int value) ? value : 0;

        var priority = prioValue switch
        {
            1 => "Tinggi",
            2 => "Sedang",
            3 => "Rendah",
            _ => null
        };

        if (priority == null)
        {
            Console.WriteLine("Pilihan tidak valid, prioritas tidak diubah.");
            return;
        }

        string timestamp = DateTime.UtcNow.ToString("O");

        using var connection = dbService.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE notes SET priority = @priority, modifiedAt = @modifiedAt WHERE id = @id";
        command.Parameters.AddWithValue("@priority", dbService.Encrypt(priority));
        command.Parameters.AddWithValue("@modifiedAt", timestamp);
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
            Console.WriteLine($"Prioritas catatan dengan ID {id} berhasil diperbarui!");
        else
            Console.WriteLine($"Catatan dengan ID {id} tidak ditemukan!");
    }
}