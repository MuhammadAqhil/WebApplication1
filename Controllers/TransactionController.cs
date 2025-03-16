using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models.Request;
using WebApplication1.Models.Response;
using WebApplication1.Services.Transaction;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/submittrxmessage")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SubmitTransaction([FromBody] TransactionRequest request)
        {
            var (validationErrors, statusCode) = _transactionService.ValidateRequest(request);

            if (validationErrors.Any())
            {
                if (validationErrors.Contains("Access Denied!"))
                    return Unauthorized(new { Result = 0, Message = "Unauthorized: Invalid credentials" });

                return BadRequest(new { Result = 0, Message = "Validation failed", Errors = validationErrors });
            }

            var response = _transactionService.ProcessTransaction(request);
            LogTransaction(request, response);
            return Ok(response);
        }



        private void LogTransaction(TransactionRequest request, TransactionResponse response)
        {
            var logEntry = new
            {
                Request = request,
                Response = response,
                Timestamp = DateTime.UtcNow
            };

            string logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            string logFilePath = Path.Combine(logDir, "transactions.log");
            System.IO.File.AppendAllText(logFilePath, JsonConvert.SerializeObject(logEntry) + Environment.NewLine);
        }

    }
}
