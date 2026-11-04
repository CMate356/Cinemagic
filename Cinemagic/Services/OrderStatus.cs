namespace Cinemagic.Services;

/// "Pending" is the "Unpaid" state. Flow: Pending -> Paid -> Shipped -> Delivered, or Cancelled before shipping.
public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Shipped = "Shipped";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";

    private static readonly Dictionary<string, string[]> Allowed = new()
    {
        [Pending] = [Paid, Cancelled],
        [Paid] = [Shipped, Cancelled],
        [Shipped] = [Delivered],
        [Delivered] = [],
        [Cancelled] = []
    };

    public static bool CanTransition(string from, string to) =>
        Allowed.TryGetValue(from, out var next) && next.Contains(to);

    public static string Label(string status) => status switch
    {
        Pending => "Neplătită",
        Paid => "Plătită",
        Shipped => "Expediată",
        Delivered => "Livrată",
        Cancelled => "Anulată",
        _ => status
    };
}