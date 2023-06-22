using SafeZone.Data;
using SafeZone.Models;
using SafeZone.Repositories;
using Stripe;
using Stripe.Checkout;

namespace SafeZone.Services
{
    public class PayementService : IPayement
    {
        string apiurl = "";
        public async Task<string> Create(Crime crime)
        {
            int price;
            if (crime.Level == DangerousLevel.High.GetHashCode())
            {
                price = Variables.HighPrice;
            }
            else if (crime.Level == DangerousLevel.Normal.GetHashCode())
            {
                price = Variables.NormalPrice;
            }
            else
            {
                price = Variables.LowPrice;
            }
            var options = new SessionCreateOptions
            {
                SuccessUrl = this.apiurl + "/checkout/session?sessionId={CHECKOUT_SESSION_ID}",
                CancelUrl = this.apiurl + "failed",
                PaymentMethodTypes = new List<string> // Only card available in test mode?
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = price,
                            Currency = "MDG",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = crime.Title,
                                Description = crime.Description
                            },
                        },
                        Quantity = 1,
                    },
                },
            };

            //var paymentIntentService = new PaymentIntentService();
            //var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            //{
            //    Amount = crimes.Length,
            //    Currency = "MDG",
            //    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            //    {
            //        Enabled = true,
            //    },
            //});

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }
    }
}
