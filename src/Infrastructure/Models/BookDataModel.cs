using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models;

[Table("books", Schema = "public")]
public class BookDataModel
{
    [Column("id")][Key]
    public required string Id { get; set; }
    [Column("title")]
    public required string Title { get; set; }
    [Column("author")]
    public required string Author { get; set; }
}
