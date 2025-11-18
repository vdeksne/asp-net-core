using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task3_V7.Data;
using Task3_V7.Models;

namespace Task3_V7.Controllers
{
    public class QueriesController : Controller
    {
        private readonly AppAdContext _db;

        public QueriesController(AppAdContext db)
        {
            _db = db;
        }

        // Landing page with links
        [HttpGet]
        public IActionResult Index() => View();

        // 1) With input: find authors by name
        [HttpGet]
        public async Task<IActionResult> AuthorSearch(string? q)
        {
            q = (q ?? string.Empty).Trim();
            var authors = await _db.Authors
                .Where(a => string.IsNullOrEmpty(q) || a.AuthorName.Contains(q))
                .OrderBy(a => a.AuthorName)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Q = q;
            return View(authors);
        }

        // 2) With input: find books by title
        [HttpGet]
        public async Task<IActionResult> BookSearch(string? q)
        {
            q = (q ?? string.Empty).Trim();
            var books = await _db.Books
                .Where(b => string.IsNullOrEmpty(q) || b.Title.Contains(q))
                .OrderBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Q = q;
            return View(books);
        }

        // 3) With input: books published from a year
        [HttpGet]
        public async Task<IActionResult> BooksByYear(int? minYear)
        {
            var books = await _db.Books
                .Where(b => !minYear.HasValue || b.YearPublished >= minYear.Value)
                .OrderBy(b => b.YearPublished).ThenBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.MinYear = minYear;
            return View(books);
        }

        // 4) No input: all authors sorted
        [HttpGet]
        public async Task<IActionResult> AllAuthors()
        {
            var authors = await _db.Authors
                .OrderBy(a => a.AuthorName)
                .AsNoTracking()
                .ToListAsync();

            return View(authors);
        }

        // 5) No input: all books sorted by year then title
        [HttpGet]
        public async Task<IActionResult> BooksSorted()
        {
            var books = await _db.Books
                .OrderBy(b => b.YearPublished).ThenBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();

            return View(books);
        }

        // 6) No input: author -> number of books
        [HttpGet]
        public async Task<IActionResult> AuthorBookCounts()
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

            return View(items);
        }
    }
}