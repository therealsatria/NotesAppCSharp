using CsvHelper;
using NotesAppCSharp.Services;
using System.Globalization;

namespace NotesAppCSharp.Functions;

public static class ExportToCsv
{
    public static void Execute(DatabaseService dbService)
    {
        string query = "SELECT id, note, priority, createdAt, modifiedAt FROM notes";
        var notes = dbService.GetNotes(query);

        using var writer = new StreamWriter("notes_export.csv");
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(notes.Select(n => new
        {
            n.Id,
            n.Text,
            n.Priority,
            CreatedAt = n.CreatedAt.ToString("O"),
            ModifiedAt = n.ModifiedAt?.ToString("O") ?? ""
        }));

        Console.WriteLine("Data berhasil diekspor ke 'notes_export.csv'!");
    }
}