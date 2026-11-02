using Cinemagic.Models;

namespace Cinemagic.Data
{
    public static class DbSeeder
    {
        private sealed record FilmSeed(string Title, string Description, int Minutes, DateTime Release,
            string Rating, string[] Genres, string Director, string[] Cast, decimal Price);

        public static void Seed(ApplicationDbContext db)
        {
            if (db.Categories.Any()) return; // baza este deja populata

            //  Categorii
            var catTickets = new Category { Name = "Movie Tickets", Description = "Tickets for screenings in our cinemas." };
            var catVideo = new Category { Name = "Home Video", Description = "Blu-ray and DVD editions." };
            var catMerch = new Category { Name = "Merch", Description = "Posters, mugs, bags and collectibles." };
            var catSnacks = new Category { Name = "Snacks & Gift Cards", Description = "Popcorn, drinks and gift cards." };

            // Genuri 
            var genres = new[] { "Thriller", "Mystery", "Drama", "Adventure", "Sci-Fi", "Action",
                                 "Comedy", "Romance", "Family", "Horror", "Animation" }
                .Select(n => new Genre { Name = n }).ToDictionary(g => g.Name);

            // Persoane (nume inventate) 
            var people = new[] { "Ilinca Ardeleanu", "Matei Corbea", "Sofia Lazarescu", "Darius Vlaicu",
                                 "Anca Dobrescu", "Victor Nanu", "Larisa Preda", "Cosmin Ghergut",
                                 "Medeea Stoenescu", "Rares Toma", "Ioana Balan", "Adrian Mocanu",
                                 "Teodora Rusu", "Gabriel Enache", "Alina Ciobanu", "Paul Dragoi" }
                .Select(n => new Person { FullName = n }).ToDictionary(p => p.FullName);

            // Filme (fictive)
            var films = new[]
            {
                new FilmSeed("Midnight Lantern", "A night-shift archivist finds a case file that was never closed.", 118,
                    new DateTime(2026, 2, 13), "N-15", new[] { "Thriller", "Mystery" }, "Ilinca Ardeleanu",
                    new[] { "Anca Dobrescu", "Victor Nanu", "Larisa Preda" }, 34.90m),
                new FilmSeed("The Last Lighthouse", "A keeper and a stranded sailor face one last storm.", 132,
                    new DateTime(2025, 11, 21), "AP-12", new[] { "Drama", "Adventure" }, "Matei Corbea",
                    new[] { "Cosmin Ghergut", "Medeea Stoenescu", "Rares Toma" }, 32.90m),
                new FilmSeed("Neon Horizons", "A courier crosses a city that never switches off its lights.", 124,
                    new DateTime(2026, 3, 27), "N-15", new[] { "Sci-Fi", "Action" }, "Darius Vlaicu",
                    new[] { "Ioana Balan", "Adrian Mocanu", "Victor Nanu" }, 36.90m),
                new FilmSeed("Paper Moon Bakery", "Two rival bakers are forced to share one tiny kitchen.", 101,
                    new DateTime(2026, 1, 9), "AG", new[] { "Comedy", "Romance" }, "Sofia Lazarescu",
                    new[] { "Teodora Rusu", "Gabriel Enache", "Anca Dobrescu" }, 29.90m),
                new FilmSeed("Echoes of Saltwater", "A family returns to a seaside town to sell the old house.", 109,
                    new DateTime(2025, 10, 17), "AP-12", new[] { "Drama" }, "Matei Corbea",
                    new[] { "Alina Ciobanu", "Paul Dragoi", "Larisa Preda" }, 29.90m),
                new FilmSeed("Operation Pigeon", "A clumsy pigeon and three kids foil a museum heist.", 95,
                    new DateTime(2026, 4, 10), "AG", new[] { "Comedy", "Family" }, "Sofia Lazarescu",
                    new[] { "Gabriel Enache", "Medeea Stoenescu", "Rares Toma" }, 27.90m),
                new FilmSeed("Crimson Static", "A radio host starts hearing a broadcast from tomorrow.", 104,
                    new DateTime(2026, 3, 6), "IM-18", new[] { "Horror", "Thriller" }, "Ilinca Ardeleanu",
                    new[] { "Larisa Preda", "Cosmin Ghergut", "Alina Ciobanu" }, 34.90m),
                new FilmSeed("Atlas of Small Wonders", "A tiny cartographer maps the world inside a garden.", 92,
                    new DateTime(2026, 2, 20), "AG", new[] { "Animation", "Family" }, "Darius Vlaicu",
                    new[] { "Ioana Balan", "Paul Dragoi", "Teodora Rusu" }, 27.90m),
            };

            var filmProducts = new List<Product>();
            foreach (var f in films)
            {
                var details = new FilmDetails
                {
                    DurationMinutes = f.Minutes,
                    ReleaseDate = f.Release,
                    AgeRating = f.Rating,
                    OriginalTitle = f.Title,
                    TrailerUrl = "https://example.com/trailers/" + Slug(f.Title)
                };
                foreach (var g in f.Genres)
                    details.FilmGenres.Add(new FilmGenre { Genre = genres[g] });
                for (int i = 0; i < f.Cast.Length; i++)
                    details.CastMembers.Add(new FilmCast { Person = people[f.Cast[i]], BillingOrder = i + 1 });
                details.Directors.Add(new FilmDirector { Person = people[f.Director] });

                filmProducts.Add(new Product
                {
                    Category = catTickets,
                    Title = f.Title,
                    Description = f.Description,
                    Price = f.Price,          // pret "de la"; pretul biletului este in Screening.Price
                    ImageUrl = Img(f.Title, 600, 900),
                    StockQuantity = 0,        // locurile sunt in Screening.AvailableSeats
                    FilmDetails = details
                });
            }

            //  Produse non-film 
            Product Item(Category c, string title, string desc, decimal price, int stock) => new Product
            {
                Category = c,
                Title = title,
                Description = desc,
                Price = price,
                StockQuantity = stock,
                ImageUrl = Img(title, 600, 600)
            };

            var video = new List<Product>();
            for (int i = 0; i < films.Length; i++)
            {
                string kind = i % 2 == 0 ? "Blu-ray" : "DVD";
                video.Add(Item(catVideo, films[i].Title + " – " + kind,
                    kind + " edition of " + films[i].Title + ".",
                    i % 2 == 0 ? 69.90m : 44.90m, 10 + i * 3));
            }

            var merch = new List<Product>
            {
                Item(catMerch, "Neon Horizons Poster A2", "Large format poster.", 39.90m, 40),
                Item(catMerch, "Midnight Lantern Poster A2", "Large format poster.", 39.90m, 35),
                Item(catMerch, "Cinemagic Tote Bag", "Cotton tote bag with the Cinemagic logo.", 44.90m, 50),
                Item(catMerch, "Film Reel Mug", "Ceramic mug, 330 ml.", 34.90m, 60),
                Item(catMerch, "Enamel Pin Set (5 pcs)", "Five collectible film-themed pins.", 29.90m, 45),
                Item(catMerch, "Clapperboard Notebook", "A5 notebook with a clapperboard cover.", 24.90m, 70),
                Item(catMerch, "Cinemagic T-Shirt", "Unisex cotton t-shirt.", 59.90m, 30),
                Item(catMerch, "Film Strip Keychain", "Metal keychain shaped like a film strip.", 14.90m, 100),
            };

            var snacks = new List<Product>
            {
                Item(catSnacks, "Popcorn – Small", "Salted popcorn, small box.", 14.90m, 200),
                Item(catSnacks, "Popcorn – Medium", "Salted popcorn, medium box.", 19.90m, 200),
                Item(catSnacks, "Popcorn – Large", "Salted popcorn, large box.", 24.90m, 200),
                Item(catSnacks, "Nachos with Cheese", "Nachos with warm cheese sauce.", 22.90m, 150),
                Item(catSnacks, "Soft Drink 0.5 L", "Cold soft drink.", 9.90m, 300),
                Item(catSnacks, "Cinema Combo", "Medium popcorn and a soft drink.", 27.90m, 150),
                Item(catSnacks, "Gift Card 50 RON", "Gift card worth 50 RON.", 50.00m, 500),
                Item(catSnacks, "Gift Card 100 RON", "Gift card worth 100 RON.", 100.00m, 500),
            };

            //  Cinematografe
            var cinemas = new[]
            {
                new Cinema { Name = "Cinemagic Bucharest", City = "Bucharest", Address = "Bd. Exemplu 10" },
                new Cinema { Name = "Cinemagic Cluj", City = "Cluj-Napoca", Address = "Str. Exemplu 25" },
                new Cinema { Name = "Cinemagic Iasi", City = "Iași", Address = "Str. Exemplu 7" },
            };

            //  Proiectii (urmatoarele 7 zile) 
            var rng = new Random(42);
            var formats = new[] { "2D", "3D", "VIP 2D" };
            var languages = new[] { "Original, subtitled", "Dubbed (RO)" };
            var hours = new[] { 14, 16, 18, 20, 22 };
            var seatOptions = new[] { 80, 120, 150 };
            var screenings = new List<Screening>();
            for (int i = 0; i < filmProducts.Count; i++)
            {
                for (int s = 0; s < 3; s++)
                {
                    string format = formats[(i + s) % formats.Length];
                    decimal extra = format == "VIP 2D" ? 15m : format == "3D" ? 5m : 0m;
                    int total = seatOptions[rng.Next(seatOptions.Length)];
                    screenings.Add(new Screening
                    {
                        Product = filmProducts[i],
                        Cinema = cinemas[(i + s) % cinemas.Length],
                        StartTime = DateTime.Today.AddDays(1 + (i + s) % 7).AddHours(hours[(i * 3 + s) % hours.Length]),
                        Format = format,
                        Language = languages[(i + s) % languages.Length],
                        Price = filmProducts[i].Price + extra,
                        TotalSeats = total,
                        AvailableSeats = total
                    });
                }
            }

            //  Livrare si coduri de reducere 
            var delivery = new List<DeliveryMethod>
            {
                new DeliveryMethod { Name = "Courier Standard (2-3 days)", Cost = 19.99m },
                new DeliveryMethod { Name = "Courier Express (next day)", Cost = 34.99m },
                new DeliveryMethod { Name = "E-ticket by email", Cost = 0m },
                new DeliveryMethod { Name = "Box-office pickup", Cost = 0m },
            };
            var discounts = new List<DiscountCode>
            {
                new DiscountCode { Code = "WELCOME10", Type = "Percentage", Value = 10m, ExpiresAt = DateTime.Today.AddDays(90), MaxUses = 100, UsedCount = 0 },
                new DiscountCode { Code = "SAVE5", Type = "Fixed", Value = 5m, ExpiresAt = DateTime.Today.AddDays(30), MaxUses = 50, UsedCount = 0 },
                new DiscountCode { Code = "EXPIRED5", Type = "Fixed", Value = 5m, ExpiresAt = DateTime.Today.AddDays(-1), MaxUses = 50, UsedCount = 0 },
                new DiscountCode { Code = "USEDUP", Type = "Percentage", Value = 15m, ExpiresAt = DateTime.Today.AddDays(30), MaxUses = 1, UsedCount = 1 },
            };

            //  Utilizatori de test 
            var admin = new User
            {
                Email = "admin@cinemagic.local",
                FullName = "Admin Cinemagic",
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };
            var customer = new User
            {
                Email = "customer@cinemagic.local",
                FullName = "Test Customer",
                Role = "Customer",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!")
            };
            var address = new Address
            {
                User = customer,
                Street = "Str. Exemplu 12",
                City = "Bucharest",
                Phone = "0712345678",
                IsDefault = true
            };

            //  O comanda demo, o recenzie si un element in wishlist 
            var boughtProduct = video[0];
            var demoOrder = new Order
            {
                User = customer,
                DeliveryMethod = delivery[0],
                OrderDate = DateTime.Now.AddDays(-2),
                Status = "Paid",
                ContactEmail = customer.Email,
                ShippingStreet = address.Street,
                ShippingCity = address.City,
                ShippingPhone = address.Phone,
                DeliveryCost = delivery[0].Cost,
                DiscountAmount = 0m,
                StripeSessionId = "cs_test_seed_0001"
            };
            demoOrder.OrderItems.Add(new OrderItem
            {
                Product = boughtProduct,
                Quantity = 1,
                UnitPrice = boughtProduct.Price
            });
            boughtProduct.StockQuantity -= 1;

            var review = new Review
            {
                User = customer,
                Product = boughtProduct,
                Rating = 5,
                Comment = "Great edition, fast delivery."
            };
            var wish = new Wishlist { User = customer, Product = filmProducts[0] };

            // Salvare 
            db.Products.AddRange(filmProducts);
            db.Products.AddRange(video);
            db.Products.AddRange(merch);
            db.Products.AddRange(snacks);
            db.Screenings.AddRange(screenings);
            db.DeliveryMethods.AddRange(delivery);
            db.DiscountCodes.AddRange(discounts);
            db.Users.AddRange(admin, customer);
            db.Addresses.Add(address);
            db.Orders.Add(demoOrder);
            db.Reviews.Add(review);
            db.Wishlists.Add(wish);
            db.SaveChanges();
        }

        private static string Slug(string s) =>
            new string(s.ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray()).Trim('-');

        
        private static string Img(string title, int w, int h) =>
            $"https://picsum.photos/seed/{Slug(title)}/{w}/{h}";
    }
}