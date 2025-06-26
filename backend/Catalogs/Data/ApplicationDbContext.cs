using Microsoft.EntityFrameworkCore;

namespace Catalogs.Data
{
    /// <summary>
    /// Represents the database context for the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Microsoft.EntityFrameworkCore.DbSet{Catalog}"/> entities.
        /// </summary>
        public required DbSet<Catalog> Catalogs { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{Group}"/> of <see cref="Group"/> entities.
        /// </summary>
        public required DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Configures the relationships between the entities in the model.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Catalog>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.IdCatalog)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Catalog>()
            .HasOne(c => c.Group)
            .WithMany(g => g.CatalogList)
            .HasForeignKey(c => c.GroupCode)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
            .HasKey(g => g.Code);


            modelBuilder.Entity<Group>()
            .HasOne(g => g.Parent)
            .WithMany(g => g.Children)
            .HasForeignKey(g => g.ParentCode)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}