using NotesAppCSharp.Services;

namespace NotesAppCSharp.Functions;

public static class SearchNotes
{
    public static void Execute(DatabaseService dbService)
    {
        Console.Write("Masukkan kata kunci untuk mencari catatan: ");
        string? keyword = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(keyword))
        {
            Console.WriteLine("Kata kunci tidak boleh kosong!");
            return;
        }

        string query = "SELECT id, note, priority, createdAt, modifiedAt FROM notes";
        var notes = dbService.GetNotes(query);

        Console.WriteLine($"\nHasil Pencarian untuk '{keyword}':");
        Console.WriteLine("| {0,-4} | {1,-60} | {2,-10} |", "ID", "Note", "Priority");
        Console.WriteLine("|------|--------------------------------------------------------------|------------|");

        bool found = false;
        foreach (var note in notes)
        {
            if (note.Text.ToLower().Contains(keyword))
            {
                found = true;
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

        if (!found)
            Console.WriteLine($"Tidak ada catatan yang cocok dengan kata kunci '{keyword}'.");
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