using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class ShowNotes
{
    public static void Execute(DatabaseService dbService, IConfiguration config)
    {
        int limit = int.TryParse(config["ShowLimit"], out int l) ? l : 10; // Gunakan indexer
        string orderBy = config["ShowOrderBy"]?.ToLower() ?? "modifiedat"; // Gunakan indexer
        string orderColumn = orderBy switch
        {
            "id" => "id DESC",
            "createdat" => "createdAt DESC",
            "modifiedat" => "modifiedAt DESC",
            _ => "createdAt DESC"
        };

        string query = $"SELECT id, note, priority, createdAt, modifiedAt FROM notes ORDER BY {orderColumn} LIMIT @limit";
        var notes = dbService.GetNotes(query, new SqliteParameter("@limit", limit));

        Console.WriteLine($"\nDaftar Catatan (Limit: {limit}, Order By: {orderBy}):");
        Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} |", "ID", "Note", "Priority");
        Console.WriteLine("|------|--------------------------------------------------------------|------------|");

        foreach (var note in notes)
        {
            var wrappedNote = WrapText(note.Text, 60);
            for (int i = 0; i < wrappedNote.Length; i++)
            {
                if (i == 0)
                    Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} |", note.Id, wrappedNote[i], note.Priority);
                else
                    Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} |", "", wrappedNote[i], "");
            }
            Console.WriteLine("|------|--------------------------------------------------------------|------------|");
        }
    }

    private static string[] WrapText(string text, int width)
    {
        var lines = new List<string>();
        while (text.Length > width)
        {
            int splitIndex = text.LastIndexOf(' ', width);
            if (splitIndex <= 0) splitIndex = width;
            lines.Add(text.Substring(0, splitIndex));
            text = text.Substring(splitIndex).TrimStart();
        }
        lines.Add(text);
        return lines.ToArray();
    }
}