namespace Domain.Models;

public class Book(string id, string title, string author)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Author { get; set; } = author;
}
