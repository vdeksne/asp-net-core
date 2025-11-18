using Task3_V7.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Use SQL Server like previous project and enable logging for diagnostics
builder.Services.AddDbContext<AppAdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>  
{
    options.Password.RequireDigit = false;  
    options.Password.RequireLowercase = false;          
    options.Password.RequireNonAlphanumeric = false;    
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppAdContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
});
var app = builder.Build();
    
// Apply migrations and seed sample data (no drop)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppAdContext>();

    // Apply pending migrations; creates DB if it doesn't exist
    db.Database.Migrate();

    if (!db.Authors.Any())
    {
        db.Authors.AddRange(
            new Author { AuthorName = "J. R. R. Tolkien", BirthDate = new DateTime(1892, 1, 3), DeathDate = new DateTime(1973, 9, 2) },
            new Author { AuthorName = "Agatha Christie", BirthDate = new DateTime(1890, 9, 15), DeathDate = new DateTime(1976, 1, 12) }
        );
    }

    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new Book { Title = "Don Quixote", AuthorName = "Miguel de Cervantes", YearPublished = 1605 },
            new Book { Title = "A Tale of Two Cities", AuthorName = "Charles Dickens", YearPublished = 1859 },
            new Book { Title = "The Lord of the Rings", AuthorName = "J.R.R. Tolkien", YearPublished = 1954 },
            new Book { Title = "The Little Prince", AuthorName = "Antoine de Saint-Exupéry", YearPublished = 1943 },
            new Book { Title = "Harry Potter and the Sorcerer’s Stone", AuthorName = "J.K. Rowling", YearPublished = 1997 },
            new Book { Title = "And Then There Were None", AuthorName = "Agatha Christie", YearPublished = 1939 },
            new Book { Title = "Dream of the Red Chamber", AuthorName = "Cao Xueqin", YearPublished = 1791 },
            new Book { Title = "She: A History of Adventure", AuthorName = "H. Rider Haggard", YearPublished = 1887 },
            new Book { Title = "The Lion, the Witch and the Wardrobe", AuthorName = "C.S. Lewis", YearPublished = 1950 },
            new Book { Title = "The Da Vinci Code", AuthorName = "Dan Brown", YearPublished = 2003 },
            new Book { Title = "Think and Grow Rich", AuthorName = "Napoleon Hill", YearPublished = 1937 },
            new Book { Title = "Harry Potter and the Deathly Hallows", AuthorName = "J.K. Rowling", YearPublished = 2007 },
            new Book { Title = "Harry Potter and the Order of the Phoenix", AuthorName = "J.K. Rowling", YearPublished = 2003 },
            new Book { Title = "Harry Potter and the Half-Blood Prince", AuthorName = "J.K. Rowling", YearPublished = 2005 },
            new Book { Title = "Harry Potter and the Goblet of Fire", AuthorName = "J.K. Rowling", YearPublished = 2000 },
            new Book { Title = "Harry Potter and the Prisoner of Azkaban", AuthorName = "J.K. Rowling", YearPublished = 1999 },
            new Book { Title = "Harry Potter and the Chamber of Secrets", AuthorName = "J.K. Rowling", YearPublished = 1998 },
            new Book { Title = "Harry Potter and the Philosopher’s Stone", AuthorName = "J.K. Rowling", YearPublished = 1997 },
            new Book { Title = "The Catcher in the Rye", AuthorName = "J.D. Salinger", YearPublished = 1951 },
            new Book { Title = "The Alchemist", AuthorName = "Paulo Coelho", YearPublished = 1988 },
            new Book { Title = "The Bridges of Madison County", AuthorName = "Robert James Waller", YearPublished = 1992 },
            new Book { Title = "Ben-Hur: A Tale of the Christ", AuthorName = "Lew Wallace", YearPublished = 1880 },
            new Book { Title = "You Can Heal Your Life", AuthorName = "Louise Hay", YearPublished = 1984 },
            new Book { Title = "The Girl with the Dragon Tattoo", AuthorName = "Stieg Larsson", YearPublished = 2005 },
            new Book { Title = "The Very Hungry Caterpillar", AuthorName = "Eric Carle", YearPublished = 1969 },
            new Book { Title = "Twilight", AuthorName = "Stephenie Meyer", YearPublished = 2005 },
            new Book { Title = "New Moon", AuthorName = "Stephenie Meyer", YearPublished = 2006 },
            new Book { Title = "Eclipse", AuthorName = "Stephenie Meyer", YearPublished = 2007 },
            new Book { Title = "Breaking Dawn", AuthorName = "Stephenie Meyer", YearPublished = 2008 },
            new Book { Title = "To Kill a Mockingbird", AuthorName = "Harper Lee", YearPublished = 1960 }
        );
    }

    db.SaveChanges();
}

// Simple endpoint to search authors by name part
app.MapGet("/authors/search", async (string? q, AppAdContext db) =>
{
    q = (q ?? string.Empty).Trim();
    var authors = await db.Authors
        .Where(a => a.AuthorName.Contains(q))
        .OrderBy(a => a.AuthorName)
        .Select(a => new { a.AuthorName, Birth = a.BirthDate, Death = a.DeathDate })
        .ToListAsync();
    return Results.Ok(authors);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Queries}/{action=Index}/{id?}");

app.Run();
