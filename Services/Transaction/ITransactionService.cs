using WebApplication1.Models.Request;
using WebApplication1.Models.Response;

namespace WebApplication1.Services.Transaction
{
    public interface ITransactionService
    {
        TransactionResponse ProcessTransaction(TransactionRequest request);
        (List<string> Errors, int StatusCode) ValidateRequest(TransactionRequest request);
    }
}
