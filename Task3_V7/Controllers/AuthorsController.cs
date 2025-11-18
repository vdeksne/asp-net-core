using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task3_V7.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Task3_V7.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly AppAdContext _db;
        public AuthorsController(AppAdContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? q)
        {
            q = (q ?? string.Empty).Trim();
            var authors = await _db.Authors
                .Where(a => a.AuthorName.Contains(q))
                .OrderBy(a => a.AuthorName)
                .ToListAsync();
            ViewData["Query"] = q;
            return View(authors);
        }
    }
}