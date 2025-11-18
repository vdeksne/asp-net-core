    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task3_V7.Data
{
    public class AppAdContext : IdentityDbContext<AppUser>
    {
        public AppAdContext(DbContextOptions<AppAdContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Books -> Characters (1:N) cascade
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Characters)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [Table("Books")]
    public class Book
    {
        [Key]
        [Column("BookID")]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Preserve original property name used in queries
        [Required]
        [StringLength(150)]
        public string AuthorName { get; set; } = string.Empty;

        public int YearPublished { get; set; }

        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }

    [Table("Characters")]
    public class Character
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Role { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        public int Age { get; set; }
    }

    [Table("Authors")]
    public class Author
    {
        [Key]
        public int Id { get; set; } // Keep simple; map if DB uses AuthorID
        [Required, StringLength(150)]
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
    }
}
