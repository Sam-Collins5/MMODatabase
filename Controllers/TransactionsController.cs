using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public IActionResult Index(string? userName, string? itemType, int? minTotal, int? maxTotal, string? sortBy)
        {
            var model = _transactionService.GetTransactions(userName, itemType, minTotal, maxTotal, sortBy);
            return View(model);
        }
    }
}
