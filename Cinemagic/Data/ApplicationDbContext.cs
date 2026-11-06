using Microsoft.EntityFrameworkCore;
using Cinemagic.Models;

namespace Cinemagic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<FilmDetails> FilmDetails { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<FilmCast> FilmCasts { get; set; }
        public DbSet<FilmDirector> FilmDirectors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<StripeEvent> StripeEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Chei compuse (tabele de legatura many-to-many) 
            modelBuilder.Entity<Wishlist>().HasKey(w => new { w.UserId, w.ProductId });
            modelBuilder.Entity<Review>().HasKey(r => new { r.UserId, r.ProductId });
            modelBuilder.Entity<FilmGenre>().HasKey(fg => new { fg.ProductId, fg.GenreId });
            modelBuilder.Entity<FilmCast>().HasKey(fc => new { fc.ProductId, fc.PersonId });
            modelBuilder.Entity<FilmDirector>().HasKey(fd => new { fd.ProductId, fd.PersonId });

            //  Relatiile noi 
            modelBuilder.Entity<FilmGenre>()
                .HasOne(fg => fg.FilmDetails).WithMany(f => f.FilmGenres)
                .HasForeignKey(fg => fg.ProductId);
            modelBuilder.Entity<FilmGenre>()
                .HasOne(fg => fg.Genre).WithMany(g => g.FilmGenres)
                .HasForeignKey(fg => fg.GenreId);

            modelBuilder.Entity<FilmCast>()
                .HasOne(fc => fc.FilmDetails).WithMany(f => f.CastMembers)
                .HasForeignKey(fc => fc.ProductId);
            modelBuilder.Entity<FilmCast>()
                .HasOne(fc => fc.Person).WithMany(p => p.CastRoles)
                .HasForeignKey(fc => fc.PersonId);

            modelBuilder.Entity<FilmDirector>()
                .HasOne(fd => fd.FilmDetails).WithMany(f => f.Directors)
                .HasForeignKey(fd => fd.ProductId);
            modelBuilder.Entity<FilmDirector>()
                .HasOne(fd => fd.Person).WithMany(p => p.DirectedFilms)
                .HasForeignKey(fd => fd.PersonId);

            //  Chei candidate (UNIQUE) 
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<DiscountCode>().HasIndex(d => d.Code).IsUnique();
            modelBuilder.Entity<Genre>().HasIndex(g => g.Name).IsUnique();
            modelBuilder.Entity<Cart>().HasIndex(c => c.UserId).IsUnique();
            modelBuilder.Entity<Order>().HasIndex(o => o.StripeSessionId).IsUnique();

            // O singura adresa implicita per utilizator
            modelBuilder.Entity<Address>()
                .HasIndex(a => a.UserId)
                .IsUnique()
                .HasFilter("\"IsDefault\" = 1")
                .HasDatabaseName("UX_Addresses_OneDefaultPerUser");

            //  CartItem / OrderItem: doua indexuri unice partiale 
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique()
                .HasFilter("\"ScreeningId\" IS NULL")
                .HasDatabaseName("UX_CartItems_Cart_Product");
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ScreeningId })
                .IsUnique()
                .HasFilter("\"ProductId\" IS NULL")
                .HasDatabaseName("UX_CartItems_Cart_Screening");

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => new { oi.OrderId, oi.ProductId })
                .IsUnique()
                .HasFilter("\"ScreeningId\" IS NULL")
                .HasDatabaseName("UX_OrderItems_Order_Product");
            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => new { oi.OrderId, oi.ScreeningId })
                .IsUnique()
                .HasFilter("\"ProductId\" IS NULL")
                .HasDatabaseName("UX_OrderItems_Order_Screening");

            //  Constrangeri CHECK 
            modelBuilder.Entity<CartItem>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_CartItems_ProductOrScreening",
                    "(\"ProductId\" IS NOT NULL AND \"ScreeningId\" IS NULL) OR (\"ProductId\" IS NULL AND \"ScreeningId\" IS NOT NULL)");
                t.HasCheckConstraint("CK_CartItems_Quantity", "\"Quantity\" > 0");
            });
            modelBuilder.Entity<OrderItem>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrderItems_ProductOrScreening",
                    "(\"ProductId\" IS NOT NULL AND \"ScreeningId\" IS NULL) OR (\"ProductId\" IS NULL AND \"ScreeningId\" IS NOT NULL)");
                t.HasCheckConstraint("CK_OrderItems_Quantity", "\"Quantity\" > 0");
            });
            modelBuilder.Entity<Screening>().ToTable(t =>
                t.HasCheckConstraint("CK_Screenings_Seats",
                    "\"AvailableSeats\" >= 0 AND \"AvailableSeats\" <= \"TotalSeats\""));
            modelBuilder.Entity<Product>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Products_Stock", "\"StockQuantity\" >= 0");
                t.HasCheckConstraint("CK_Products_Price", "\"Price\" >= 0");
            });
            modelBuilder.Entity<Review>().ToTable(t =>
                t.HasCheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5"));
            modelBuilder.Entity<DiscountCode>().ToTable(t =>
                t.HasCheckConstraint("CK_DiscountCodes_Type", "\"Type\" IN ('Fixed', 'Percentage')"));

            //  Istoricul comenzilor nu se sterge in cascada 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User).WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryMethod).WithMany(d => d.Orders)
                .HasForeignKey(o => o.DeliveryMethodId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product).WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Screening).WithMany()
                .HasForeignKey(oi => oi.ScreeningId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}