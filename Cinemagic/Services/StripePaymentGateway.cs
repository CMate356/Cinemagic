using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
// Stripe.net also has types called Order, Product and Address: make "Order" mean our model.
using Order = Cinemagic.Models.Order;

namespace Cinemagic.Services;

public class StripeOptions
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty; // used from week 9
}

public record CheckoutSessionInfo(string Id, string Url);

public interface IPaymentGateway
{
    /// The order must have its OrderItems loaded: Order.TotalAmount is calculated from them.
    Task<CheckoutSessionInfo> CreateCheckoutSessionAsync(
        Order order, string successUrl, string cancelUrl, CancellationToken ct = default);
}

/// Stripe Checkout in test mode.
public class StripePaymentGateway : IPaymentGateway
{
    private readonly IOptions<StripeOptions> _options;
    private StripeClient? _clientInstance;

    public StripePaymentGateway(IOptions<StripeOptions> options)
    {
        _options = options;
    }

    // Created on first use so the app still starts when the key is not configured yet.
    private StripeClient _client
    {
        get
        {
            if (_clientInstance is null)
            {
                var key = _options.Value.SecretKey;
                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidOperationException("Stripe:SecretKey is not configured (use dotnet user-secrets).");
                _clientInstance = new StripeClient(key);
            }
            return _clientInstance;
        }
    }

    public async Task<CheckoutSessionInfo> CreateCheckoutSessionAsync(
        Order order, string successUrl, string cancelUrl, CancellationToken ct = default)
    {
        if (order.OrderItems.Count == 0)
            throw new InvalidOperationException("Load the order with its OrderItems: TotalAmount is calculated from them.");

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Locale = "ro",
            ClientReferenceId = order.Id.ToString(),
            CustomerEmail = order.ContactEmail,
            Metadata = new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            // One line with the final total (items + delivery - discount), calculated on our server.
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "ron",
                        UnitAmount = (long)Math.Round(order.TotalAmount * 100m, MidpointRounding.AwayFromZero),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Comanda Cinemagic #{order.Id}"
                        }
                    }
                }
            }
        };

        var session = await new SessionService(_client).CreateAsync(options, cancellationToken: ct);
        return new CheckoutSessionInfo(session.Id, session.Url);
    }
}