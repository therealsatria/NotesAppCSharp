using Microsoft.Extensions.Configuration;
using NotesAppCSharp.Services;
using NotesAppCSharp.Functions;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string? encryptionKey = config["EncryptionKey"];
        if (string.IsNullOrEmpty(encryptionKey))
        {
            Console.WriteLine("Error: EncryptionKey tidak ditemukan di appsettings.json.");
            return;
        }
        if (encryptionKey.Length != 32)
        {
            Console.WriteLine($"Error: EncryptionKey harus 32 karakter, saat ini: {encryptionKey.Length}.");
            return;
        }

        byte[] key = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
        var encryptionService = new EncryptionService(key);
        var dbService = new DatabaseService("notes.db", encryptionService);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nSimple Notes App");
            Console.WriteLine("1. Tambah Catatan");
            Console.WriteLine("2. Tampilkan Catatan (Dengan Limit)");
            Console.WriteLine("3. Hapus Catatan");
            Console.WriteLine("4. Edit Catatan");
            Console.WriteLine("5. Refresh Data");
            Console.WriteLine("6. Lihat Catatan Berdasarkan ID");
            Console.WriteLine("7. Export ke CSV");
            Console.WriteLine("8. Import dari CSV");
            Console.WriteLine("9. Search Catatan");
            Console.WriteLine("10. Tampilkan Semua Catatan");
            Console.WriteLine("11. Keluar");
            Console.Write("Pilih opsi (1-11): ");

            string? choice = Console.ReadLine();
            if (!int.TryParse(choice, out int option) || option < 1 || option > 11)
            {
                Console.WriteLine("Pilihan tidak valid!");
                continue;
            }

            switch (option)
            {
                case 1: AddNote.Execute(dbService); break;
                case 2: ShowNotes.Execute(dbService, config); break; // Pastikan 2 argumen
                case 3: DeleteNote.Execute(dbService); break;
                case 4: EditNote.Execute(dbService, null); break;
                case 5: RefreshData.Execute(dbService, config); break; // Pastikan 2 argumen
                case 6: ViewNoteById.Execute(dbService); break;
                case 7: ExportToCsv.Execute(dbService); break;
                case 8: ImportFromCsv.Execute(dbService); break;
                case 9: SearchNotes.Execute(dbService); break;
                case 10: ShowAllNotes.Execute(dbService); break;
                case 11:
                    Console.WriteLine("Keluar dari aplikasi.");
                    exit = true;
                    break;
            }
        }
    }
}