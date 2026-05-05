using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class TransactionsIndexViewModel
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        // Filter state (so the form remembers what the user picked)
        public string? FilterUserName { get; set; }
        public string FilterItemType { get; set; } = "All";
        public int? FilterMinTotal { get; set; }
        public int? FilterMaxTotal { get; set; }
        public string SortBy { get; set; } = "Newest";

        // Summary stats
        public int TotalCount { get; set; }
        public int GrandTotal { get; set; }
    }
}
