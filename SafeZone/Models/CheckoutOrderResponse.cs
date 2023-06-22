namespace SafeZone.Models
{
    public class CheckoutOrderResponse
    {
        public string? SessionId { get; set; }
        public Crime Crime { get; set; }
        public DateTime Paid_at { get; set; }
    }
}
