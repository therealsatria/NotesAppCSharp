using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class ShowAllNotes
{
    public static void Execute(DatabaseService dbService)
    {
        string query = "SELECT id, note, priority, createdAt, modifiedAt FROM notes ORDER BY id ASC";
        var notes = dbService.GetNotes(query);

        Console.WriteLine("\nDaftar Semua Catatan (Urut berdasarkan ID):");
        Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} | {3,-19} | {4,-19} |",
            "ID", "Note", "Priority", "Created At", "Modified At");
        Console.WriteLine("|------|--------------------------------------------------------------|------------|---------------------|---------------------|");

        foreach (var note in notes)
        {
            var wrappedNote = WrapText(note.Text, 60);
            for (int i = 0; i < wrappedNote.Length; i++)
            {
                if (i == 0)
                    Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} | {3,-19} | {4,-19} |",
                        note.Id,
                        wrappedNote[i],
                        note.Priority,
                        note.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        note.ModifiedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                else
                    Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} | {3,-19} | {4,-19} |",
                        "", wrappedNote[i], "", "", "");
            }
            Console.WriteLine("|------|--------------------------------------------------------------|------------|---------------------|---------------------|");
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