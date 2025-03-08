namespace NotesAppCSharp.Models;

public class Note
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}