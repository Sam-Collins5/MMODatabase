using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IMongoCollection<Transaction> _transactions;

        public TransactionService()
        {
            _transactions = MongoConnection.Database.GetCollection<Transaction>("Transactions");
        }

        public List<Transaction> GetAllTransactions()
        {
            return _transactions.Find(_ => true).ToList();
        }

        public TransactionsIndexViewModel GetTransactions(string? userName, string? itemType, int? minTotal, int? maxTotal, string? sortBy)
        {
            var filterBuilder = Builders<Transaction>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                filter &= filterBuilder.Regex(t => t.UserName,
                    new MongoDB.Bson.BsonRegularExpression(userName, "i"));
            }

            if (!string.IsNullOrWhiteSpace(itemType) && itemType != "All")
            {
                filter &= filterBuilder.Eq(t => t.ItemType, itemType);
            }

            if (minTotal.HasValue)
            {
                filter &= filterBuilder.Gte(t => t.Total, minTotal.Value);
            }

            if (maxTotal.HasValue)
            {
                filter &= filterBuilder.Lte(t => t.Total, maxTotal.Value);
            }

            var sortBuilder = Builders<Transaction>.Sort;
            SortDefinition<Transaction> sortDef = sortBy switch
            {
                "TotalAsc"      => sortBuilder.Ascending(t => t.Total),
                "TotalDesc"     => sortBuilder.Descending(t => t.Total),
                "PlayerAsc"     => sortBuilder.Ascending(t => t.UserName),
                "PlayerDesc"    => sortBuilder.Descending(t => t.UserName),
                "DateAsc"       => sortBuilder.Ascending(t => t.TransactionDate),
                _               => sortBuilder.Descending(t => t.TransactionId)
            };

            var results = _transactions.Find(filter).Sort(sortDef).ToList();

            return new TransactionsIndexViewModel
            {
                Transactions = results,
                FilterUserName = userName,
                FilterItemType = itemType ?? "All",
                FilterMinTotal = minTotal,
                FilterMaxTotal = maxTotal,
                SortBy = sortBy ?? "Newest",
                TotalCount = results.Count,
                GrandTotal = results.Sum(t => t.Total)
            };
        }
    }
}