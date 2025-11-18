using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task3_V7.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Task3_V7.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppAdContext _db;
        public HomeController(AppAdContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            q = (q ?? string.Empty).Trim();
            var books = await _db.Books
                .Where(b => q == string.Empty || b.AuthorName.Contains(q))
                .OrderBy(b => b.AuthorName)
                .ThenBy(b => b.Title)
                .ToListAsync();
            ViewData["Query"] = q;
            return View(books);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            // Provide a simple redirect so both / (home) and /Home/Privacy can be used
            return View();
        }
    }
}
