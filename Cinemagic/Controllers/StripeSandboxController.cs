using Cinemagic.Data;
using Cinemagic.Models;
using Cinemagic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Controllers
{
    /// Development-only page for week 6: creates one Pending test order from the seeded data
    /// and sends you to Stripe's test payment page. Delete this controller after the demo.
    public class StripeSandboxController(
        ApplicationDbContext db,
        IPaymentGateway gateway,
        IWebHostEnvironment env) : Controller
    {
        [HttpGet("stripe-sandbox/start")]
        public async Task<IActionResult> Start()
        {
            if (!env.IsDevelopment()) return NotFound();

            // The seeded test customer (any Customer works if you registered your own).
            var user = await db.Users.AsNoTracking()
                .Where(u => u.Role == "Customer").OrderBy(u => u.Id).FirstOrDefaultAsync();
            // SQLite cannot ORDER BY a decimal column, so the (tiny) list is sorted in memory.
            var delivery = (await db.DeliveryMethods.AsNoTracking().ToListAsync())
                .OrderBy(d => d.Cost).FirstOrDefault();
            var product = await db.Products.AsNoTracking()
                .Where(p => p.IsActive && p.StockQuantity > 0).OrderBy(p => p.Id).FirstOrDefaultAsync();

            if (user is null || delivery is null || product is null)
                return Content("The test needs at least one Customer user, one delivery method and one product with stock. Start the app once so the seeder runs.");

            var address = await db.Addresses.AsNoTracking()
                .Where(a => a.UserId == user.Id).OrderByDescending(a => a.IsDefault).FirstOrDefaultAsync();

            var order = new Order
            {
                UserId = user.Id,
                DeliveryMethodId = delivery.Id,
                Status = OrderStatus.Pending,
                ContactEmail = user.Email,
                ShippingStreet = address?.Street ?? "Str. Test 1",
                ShippingCity = address?.City ?? "Bucharest",
                ShippingPhone = address?.Phone ?? "0700000000",
                DeliveryCost = delivery.Cost,
                DiscountAmount = 0m,
                // Subtotal and TotalAmount are calculated from the items, they are not columns.
                OrderItems = { new OrderItem { ProductId = product.Id, Quantity = 1, UnitPrice = product.Price } }
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var successUrl = Url.Action(nameof(Success), "StripeSandbox", new { id = order.Id }, Request.Scheme)!;
            var cancelUrl = Url.Action(nameof(Cancelled), "StripeSandbox", new { id = order.Id }, Request.Scheme)!;

            var session = await gateway.CreateCheckoutSessionAsync(order, successUrl, cancelUrl);

            order.StripeSessionId = session.Id;
            await db.SaveChangesAsync();

            return Redirect(session.Url);
        }

        [HttpGet("stripe-sandbox/success/{id:int}")]
        public async Task<IActionResult> Success(int id)
        {
            if (!env.IsDevelopment()) return NotFound();
            var status = await db.Orders.AsNoTracking().Where(o => o.Id == id).Select(o => o.Status).FirstOrDefaultAsync();
            return Content($"Stripe finished. Order #{id} status in the database: {status}. " +
                           "It stays Pending: only the signed webhook (week 9) marks an order Paid.");
        }

        [HttpGet("stripe-sandbox/cancelled/{id:int}")]
        public async Task<IActionResult> Cancelled(int id)
        {
            if (!env.IsDevelopment()) return NotFound();
            var status = await db.Orders.AsNoTracking().Where(o => o.Id == id).Select(o => o.Status).FirstOrDefaultAsync();
            return Content($"Payment cancelled. Order #{id} status in the database: {status} (unpaid).");
        }
    }
}