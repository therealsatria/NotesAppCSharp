using Microsoft.Extensions.Configuration;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class RefreshData
{
    public static void Execute(DatabaseService dbService, IConfiguration config)
    {
        ShowNotes.Execute(dbService, config); // Sertakan config
        Console.WriteLine("Data telah diperbarui.");
    }
}