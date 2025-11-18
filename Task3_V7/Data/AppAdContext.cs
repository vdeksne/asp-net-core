using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Task3_V7.Data
{
    public class AppAdContext: IdentityDbContext<AppUser>
    {
        public AppAdContext(DbContextOptions<AppAdContext> options)
            : base(options)
        {
        }

        // DbSets added
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map Book primary key to existing DB column name if schema uses BookID
            modelBuilder.Entity<Book>()
                .Property(b => b.Id)
                .HasColumnName("BookID");

            // Books -> Characters (1:N) cascade
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Characters)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    // Entities migrated from previous console app (simplified)
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty; // store author name if not linking
        public int YearPublished { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }

    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Role { get; set; } = string.Empty;
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
    }
}
