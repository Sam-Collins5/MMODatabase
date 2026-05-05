using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface ITransactionService
    {
        TransactionsIndexViewModel GetTransactions(string? playerName, string? itemType, int? minTotal, int? maxTotal, string? sortBy);
        List<Transaction> GetAllTransactions();
    }
}
