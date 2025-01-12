using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BookRepository
{
    private readonly AppDbContext dbContext;

    public BookRepository(string connectionString)
    {
        dbContext = new AppDbContext(connectionString);
    }

    public async Task<IEnumerable<BookDataModel>> GetBooksAsync()
    {
        var books = dbContext.Books ?? throw new Exception("Failed to get books");
        return await books.ToListAsync();
    }
}
