using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task3_V7.Data;
using Task3_V7.Models;

namespace Task3_V7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksApiController : ControllerBase
    {
        private readonly AppAdContext _db;
        public BooksApiController(AppAdContext db)
        {
            _db = db;
        }

        // POST: api/booksapi/author-search
        // Body: { "q": "tolkien" }
        [HttpPost("author-search")]
        public async Task<ActionResult<IEnumerable<Author>>> AuthorSearch([FromBody] SearchRequest req)
        {
            var q = (req?.Q ?? string.Empty).Trim();
            var authors = await _db.Authors
                .Where(a => string.IsNullOrEmpty(q) || a.AuthorName.Contains(q))
                .OrderBy(a => a.AuthorName)
                .AsNoTracking()
                .ToListAsync();
            return Ok(authors);
        }

        // POST: api/booksapi/book-search
        // Body: { "q": "hobbit" }
        [HttpPost("book-search")]
        public async Task<ActionResult<IEnumerable<Book>>> BookSearch([FromBody] SearchRequest req)
        {
            var q = (req?.Q ?? string.Empty).Trim();
            var books = await _db.Books
                .Where(b => string.IsNullOrEmpty(q) || b.Title.Contains(q))
                .OrderBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();
            return Ok(books);
        }

        // POST: api/booksapi/books-by-year
        // Body: { "minYear": 1950 }
        [HttpPost("books-by-year")]
        public async Task<ActionResult<IEnumerable<Book>>> BooksByYear([FromBody] YearRequest req)
        {
            int? minYear = req?.MinYear;
            var books = await _db.Books
                .Where(b => !minYear.HasValue || b.YearPublished >= minYear.Value)
                .OrderBy(b => b.YearPublished).ThenBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();
            return Ok(books);
        }

        // GET: api/booksapi/authors
        [HttpGet("authors")]
        public async Task<ActionResult<IEnumerable<Author>>> AllAuthors()
        {
            var authors = await _db.Authors
                .OrderBy(a => a.AuthorName)
                .AsNoTracking()
                .ToListAsync();
            return Ok(authors);
        }

        // GET: api/booksapi/books-sorted
        [HttpGet("books-sorted")]
        public async Task<ActionResult<IEnumerable<Book>>> BooksSorted()
        {
            var books = await _db.Books
                .OrderBy(b => b.YearPublished).ThenBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();
            return Ok(books);
        }

        // GET: api/booksapi/author-book-counts
        [HttpGet("author-book-counts")]
        public async Task<ActionResult<IEnumerable<AuthorBookCountVM>>> AuthorBookCounts()
        {
            var items = await _db.Books
                .GroupBy(b => b.AuthorName)
                .Select(g => new AuthorBookCountVM
                {
                    AuthorName = g.Key,
                    BookCount = g.Count()
                })
                .OrderByDescending(x => x.BookCount).ThenBy(x => x.AuthorName)
                .ToListAsync();
            return Ok(items);
        }

        // DELETE: api/booksapi/books/{id}
        [HttpDelete("books/{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
