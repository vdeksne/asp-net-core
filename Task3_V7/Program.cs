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
            new Book { Title = "The Hobbit", AuthorName = "J. R. R. Tolkien", YearPublished = 1937 },
            new Book { Title = "The Fellowship of the Ring", AuthorName = "J. R. R. Tolkien", YearPublished = 1954 },
            new Book { Title = "Murder on the Orient Express", AuthorName = "Agatha Christie", YearPublished = 1934 }
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
