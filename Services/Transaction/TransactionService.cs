using WebApplication1.Models.Request;
using WebApplication1.Models.Response;

namespace WebApplication1.Services.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private static readonly Dictionary<string, string> AllowedPartners = new()
        {
            { "FAKEGOOGLE", "FAKEPASSWORD1234" },
            { "FAKEPEOPLE", "FAKEPASSWORD4578" }
        };

        public TransactionService(ILogger<TransactionService> logger)
        {
            _logger = logger;
        }

        public TransactionResponse ProcessTransaction(TransactionRequest request)
        {
            var (validationErrors, statusCode) = ValidateRequest(request);

            if (validationErrors.Any())
            {
                return new TransactionResponse { Result = 0, ResultMessage = string.Join(", ", validationErrors) };
            }

            long discount = CalculateDiscount(request.TotalAmount / 100);
            long finalAmount = request.TotalAmount - discount;

            return new TransactionResponse
            {
                Result = 1,
                TotalAmount = request.TotalAmount,
                TotalDiscount = discount,
                FinalAmount = finalAmount
            };
        }


        public (List<string> Errors, int StatusCode) ValidateRequest(TransactionRequest request)
        {
            var errors = new List<string>();

            // Validate PartnerKey
            if (string.IsNullOrEmpty(request.PartnerKey) || !AllowedPartners.ContainsKey(request.PartnerKey))
            {
                return (new List<string> { "Access Denied!" }, 401);
            }

            if (string.IsNullOrEmpty(request.PartnerRefNo))
            {
                errors.Add("partnerrefno is Required.");
            }


            if (request.Items != null && request.Items.Any())
            {
                long calculatedTotal = request.Items.Sum(item => item.Qty * item.UnitPrice);
                if (calculatedTotal != request.TotalAmount)
                {
                    errors.Add("Invalid Total Amount.");
                }
            }


            long requestTimestamp = new DateTimeOffset(request.Timestamp).ToUnixTimeSeconds();
            DateTime serverTime = DateTime.UtcNow;
            DateTime requestTime = DateTimeOffset.FromUnixTimeSeconds(requestTimestamp).UtcDateTime;

            if (Math.Abs((serverTime - requestTime).TotalMinutes) > 5)
            {
                errors.Add("Expired.");
            }

            if (request.Items != null)
            {
                foreach (var item in request.Items)
                {
                    if (string.IsNullOrEmpty(item.PartnerItemRef)) errors.Add("partneritemref is Required.");
                    if (string.IsNullOrEmpty(item.Name)) errors.Add("Item name is Required.");
                    if (item.Qty < 1 || item.Qty > 5) errors.Add("Invalid item quantity. Must be between 1 and 5.");
                    if (item.UnitPrice <= 0) errors.Add("Invalid item price.");
                }
            }

            return errors.Any() ? (errors, 400) : (new List<string>(), 200);
        }




        private long CalculateDiscount(long totalAmount)
        {
            double discount = 0;
            

            if (totalAmount < 200)
            {
                discount = 0;
            }
            else if (totalAmount <= 500)
            {
                discount = 0.05;
            }
            else if (totalAmount <= 800)
            {
                discount = 0.07;
            }
            else if (totalAmount <= 1200)
            {
                discount = 0.10;
            }
            else {
                discount = 0.15;
            }

            if (totalAmount > 500)
            {
                bool isPrime = totalAmount > 1;
                for (long i = 2; i * i <= totalAmount && isPrime; i++)
                {
                    if (totalAmount % i == 0)
                        isPrime = false;
                }
                if (isPrime) discount += 0.08;
            }

            
            if (totalAmount > 900 && totalAmount % 10 == 5)
            {
                discount += 0.10;
            }


            discount = Math.Min(discount, 0.20);

            return (long)(totalAmount * discount *100);
        }

    }

}
