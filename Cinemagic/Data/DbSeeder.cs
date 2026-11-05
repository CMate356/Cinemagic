using Cinemagic.Models;

namespace Cinemagic.Data
{
    public static class DbSeeder
    {
        private sealed record FilmSeed(string Title, string Description, int Minutes, DateTime Release,
            string Rating, string[] Genres, string Director, string[] Cast, decimal Price, string ImageURL);

        public static void Seed(ApplicationDbContext db)
        {
            if (db.Categories.Any()) return; 

            // Categories
            var catTickets = new Category { Name = "Movie Tickets", Description = "Tickets for screenings in our cinemas." };
            var catVideo = new Category { Name = "Home Video", Description = "Blu-ray and DVD editions." };
            var catMerch = new Category { Name = "Merch", Description = "Posters, mugs, bags and collectibles." };
            var catSnacks = new Category { Name = "Snacks & Gift Cards", Description = "Popcorn, drinks and gift cards." };

            // Genres
            var genreNames = new[] { "Thriller", "Mystery", "Drama", "Adventure", "Sci-Fi", "Action", "Comedy", "Romance", "Family", "Horror", "Animation" };
            var genres = genreNames.Select(n => new Genre { Name = n }).ToDictionary(g => g.Name);

            // People
            var people = new[] { "Ilinca Ardeleanu", "Matei Corbea", "Sofia Lazarescu", "Darius Vlaicu",
                                 "Anca Dobrescu", "Victor Nanu", "Larisa Preda", "Cosmin Ghergut",
                                 "Medeea Stoenescu", "Rares Toma", "Ioana Balan", "Adrian Mocanu",
                                 "Teodora Rusu", "Gabriel Enache", "Alina Ciobanu", "Paul Dragoi" }
                .Select(n => new Person { FullName = n }).ToDictionary(p => p.FullName);

            // Movie Catalog
            var films = new[]
            {
                new FilmSeed("Baiat de Milioane", "A comedy full of twists and hearty humor.", 100, new DateTime(2026, 2, 13), "AP-12", new[] { "Comedy" }, "Ilinca Ardeleanu", new[] { "Anca Dobrescu", "Victor Nanu", "Larisa Preda" }, 34.90m, "/images/Baiat_de_milioane.jpg"),
                new FilmSeed("Other Mommy", "A dark psychological thriller that tests the limits of your courage.", 93, new DateTime(2025, 11, 21), "N-15", new[] { "Horror", "Thriller" }, "Matei Corbea", new[] { "Cosmin Ghergut", "Medeea Stoenescu", "Rares Toma" }, 32.90m, "/images/Cealalta_Mama.jpg"),
                new FilmSeed("Clayface", "Intense action and mystery in an unforgiving metropolis.", 105, new DateTime(2026, 3, 27), "N-15", new[] { "Action", "Horror", "Thriller" }, "Darius Vlaicu", new[] { "Ioana Balan", "Adrian Mocanu", "Victor Nanu" }, 36.90m, "/images/Clayface.jpg"),
                new FilmSeed("Nightborn", "A mysterious drama about old secrets and shadows from the past.", 92, new DateTime(2026, 1, 9), "IM-18", new[] { "Drama", "Horror" }, "Sofia Lazarescu", new[] { "Teodora Rusu", "Gabriel Enache", "Anca Dobrescu" }, 29.90m, "/images/Copilul_Noptii.jpg"),
                new FilmSeed("Craciun Amar", "A touching drama about family, choices, and forgiveness.", 111, new DateTime(2025, 10, 17), "AP-12", new[] { "Drama" }, "Matei Corbea", new[] { "Alina Ciobanu", "Paul Dragoi", "Larisa Preda" }, 29.90m, "/images/Craciun_Amar.jpg"),
                new FilmSeed("Primetime", "A captivating dive into the backstage of mass media and justice.", 110, new DateTime(2026, 4, 10), "N-15", new[] { "Action", "Drama" }, "Sofia Lazarescu", new[] { "Gabriel Enache", "Medeea Stoenescu", "Rares Toma" }, 27.90m, "/images/Primetime.jpg"),
                new FilmSeed("Ratiune si Simtire", "A classic tale of love and drama in a spectacular setting.", 131, new DateTime(2026, 3, 6), "AP-12", new[] { "Drama", "Romance" }, "Ilinca Ardeleanu", new[] { "Larisa Preda", "Cosmin Ghergut", "Alina Ciobanu" }, 34.90m, "/images/Ratiune_si_Simtire.jpg"),
                new FilmSeed("Forgotten Island", "An unforgettable animated adventure for all kids and the whole family.", 109, new DateTime(2026, 2, 20), "AG", new[] { "Animation", "Family", "Adventure" }, "Darius Vlaicu", new[] { "Ioana Balan", "Paul Dragoi", "Teodora Rusu" }, 27.90m, "/images/Insula_Pierduta.jpg"),
                new FilmSeed("Delta Salbatica", "A fascinating documentary about the nature and wildlife of the Danube Delta.", 105, new DateTime(2026, 5, 12), "AG", new[] { "Adventure", "Family" }, "Darius Vlaicu", new[] { "Victor Nanu", "Ioana Balan", "Rares Toma" }, 28.90m, "/images/Delta_Salbatica.jpg")
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
                    Price = f.Price,
                    ImageUrl = f.ImageURL,
                    StockQuantity = 0,
                    FilmDetails = details
                });
            }

            // Non-film products
            Product Item(Category c, string title, string desc, decimal price, int stock, string imageURL = null) => new Product
            {
                Category = c,
                Title = title,
                Description = desc,
                Price = price,
                StockQuantity = stock,
                ImageUrl = imageURL ?? Img(title, 600, 600)
            };

            var video = new List<Product>();
            for (int i = 0; i < films.Length; i++)
            {
                string kind = i % 2 == 0 ? "Blu-ray" : "DVD";
                video.Add(Item(catVideo, films[i].Title + " – " + kind, kind + " edition of " + films[i].Title + ".", i % 2 == 0 ? 69.90m : 44.90m, 10 + i * 3));
            }

            var merch = new List<Product>
            {
                Item(catMerch, "Neon Horizons Poster A2", "Large format poster.", 39.90m, 40, "/images/Poster.png"),
                Item(catMerch, "Cinemagic Tote Bag", "Cotton tote bag with the Cinemagic logo.", 44.90m, 50, "/images/totebag.png"),
                Item(catMerch, "Film Reel Mug", "Ceramic mug, 330 ml.", 34.90m, 60, "/images/mug.png"),
                Item(catMerch, "Clapperboard Notebook", "A5 notebook with a clapperboard cover.", 24.90m, 70, "/images/notebook.png"),
                Item(catMerch, "Cinemagic T-Shirt", "Unisex cotton t-shirt.", 59.90m, 30, "/images/t-shirt.png")
            };

            var snacks = new List<Product>
            {
                Item(catSnacks, "Popcorn – Small", "Salted popcorn, small box.", 14.90m, 200,"/images/PopcornS.png" ),
                Item(catSnacks, "Popcorn – Medium", "Salted popcorn, medium box.", 19.90m, 200, "/images/PopcornM.png"),
                Item(catSnacks, "Popcorn – Large", "Salted popcorn, large box.", 24.90m, 200, "/images/PopcornL.png"),
                Item(catSnacks, "Nachos with Cheese", "Nachos with warm cheese sauce.", 22.90m, 150, "/images/Nachos.png"),
                Item(catSnacks, "Apă Plată 0.5 L", "Still mineral water.", 7.90m, 300, "/images/apa_plata.png"),
                Item(catSnacks, "Apă Minerală 0.5 L", "Sparkling mineral water.", 7.90m, 300, "/images/apa_minerala.png"),
                Item(catSnacks, "Cola 0.5 L", "Refreshing cola soft drink.", 9.90m, 300, "/images/cola.png"),
                Item(catSnacks, "Cola Zero 0.5 L", "Zero sugar cola soft drink.", 9.90m, 300, "/images/cola_zero.png"),
                Item(catSnacks, "Fanta 0.5 L", "Orange flavored soft drink.", 9.90m, 300, "/images/fanta.png"),
                Item(catSnacks, "Sprite 0.5 L", "Lemon-lime flavored soft drink.", 9.90m, 300, "/images/sprite.png"),
                Item(catSnacks, "Cinema Combo", "Medium popcorn and a soft drink.", 27.90m, 150, "/images/Cinema_Combo.png")
            };

            // Cinemas
            var cinemas = new[]
            {
                new Cinema { Name = "Cinemagic Bucharest", City = "Bucharest", Address = "Bd. Exemplu 10" },
                new Cinema { Name = "Cinemagic Cluj", City = "Cluj-Napoca", Address = "Str. Exemplu 25" },
                new Cinema { Name = "Cinemagic Iasi", City = "Iași", Address = "Str. Exemplu 7" }
            };

            // Generate screenings
            var rng = new Random(42);
            var formats = new[] { "2D", "3D", "VIP 2D" };
            var languages = new[] { "Original, subtitled", "Dubbed (RO)", "English" };
            var seatOptions = new[] { 80, 120, 150 };
            var screenings = new List<Screening>();

            for (int day = 0; day < 14; day++)
            {
                var targetDate = DateTime.Today.AddDays(day);

                foreach (var cinema in cinemas)
                {
                    for (int i = 0; i < filmProducts.Count; i++)
                    {
                        var film = filmProducts[i];
                        var seedData = films[i]; // Luăm datele originale din array-ul de seed

                        // Verificăm automat după genuri (dacă e Animație sau Familie)
                        bool isKidsFilm = seedData.Genres.Contains("Animation") || seedData.Genres.Contains("Family");

                        List<int> scheduledHours = new List<int>();

                        if (isKidsFilm)
                        {
                            // Matineu copii (dimineața / prânz)
                            scheduledHours.Add(11 + ((day + i) % 2)); // 11:00 sau 12:00
                            if ((day + i) % 2 == 0) scheduledHours.Add(14); // 14:00
                        }
                        else
                        {
                            // Filme generale (după-amiază / seară)
                            int baseHour = 15 + ((i + day + cinema.Name.Length) % 6);
                            scheduledHours.Add(baseHour);

                            if ((i + day) % 2 == 0)
                            {
                                int lateHour = baseHour + 3 > 22 ? 22 : baseHour + 3;
                                if (!scheduledHours.Contains(lateHour)) scheduledHours.Add(lateHour);
                            }
                        }

                        foreach (var hour in scheduledHours)
                        {
                            string format = isKidsFilm ? "2D" : formats[(i + day + hour) % formats.Length];
                            decimal extra = format == "VIP 2D" ? 15m : format == "3D" ? 5m : 0m;
                            int totalSeats = seatOptions[rng.Next(seatOptions.Length)];
                            string lang = isKidsFilm ? "Dubbed (RO)" : languages[(i + day) % languages.Length];

                            screenings.Add(new Screening
                            {
                                Product = film,
                                Cinema = cinema,
                                StartTime = targetDate.AddHours(hour).AddMinutes((i * 15) % 60),
                                Format = format,
                                Language = lang,
                                Price = film.Price + extra,
                                TotalSeats = totalSeats,
                                AvailableSeats = totalSeats
                            });
                        }
                    }
                }
            }

            // Delivery and discount codes
            var delivery = new List<DeliveryMethod>
            {
                new DeliveryMethod { Name = "Courier Standard (2-3 days)", Cost = 19.99m },
                new DeliveryMethod { Name = "Courier Express (next day)", Cost = 34.99m },
                new DeliveryMethod { Name = "E-ticket by email", Cost = 0m },
                new DeliveryMethod { Name = "Box-office pickup", Cost = 0m }
            };

            var discounts = new List<DiscountCode>
            {
                new DiscountCode { Code = "WELCOME10", Type = "Percentage", Value = 10m, ExpiresAt = DateTime.Today.AddDays(90), MaxUses = 100, UsedCount = 0 },
                new DiscountCode { Code = "SAVE5", Type = "Fixed", Value = 5m, ExpiresAt = DateTime.Today.AddDays(30), MaxUses = 50, UsedCount = 0 }
            };

            // Demo users
            var admin = new User { Email = "admin@cinemagic.local", FullName = "Admin Cinemagic", Role = "Admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!") };
            var customer = new User { Email = "customer@cinemagic.local", FullName = "Test Customer", Role = "Customer", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!") };
            var address = new Address { User = customer, Street = "Str. Exemplu 12", City = "Bucharest", Phone = "0712345678", IsDefault = true };

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
            demoOrder.OrderItems.Add(new OrderItem { Product = boughtProduct, Quantity = 1, UnitPrice = boughtProduct.Price });

            var review = new Review { User = customer, Product = boughtProduct, Rating = 5, Comment = "Great edition, fast delivery." };
            var wish = new Wishlist { User = customer, Product = filmProducts[0] };

            // Save to DB
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