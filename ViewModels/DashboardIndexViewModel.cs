namespace MMOngo.ViewModels
{
    public class DashboardIndexViewModel
    {
        public int TotalPlayers { get; set; }
        public int TotalCharacters { get; set; }
        public int TotalGuilds { get; set; }
        public int TotalTransactions { get; set; }

        public double AverageCharacterLevel { get; set; }

        // Top guild by member count
        public string TopGuildName { get; set; } = string.Empty;
        public int TopGuildMemberCount { get; set; }

        // Most-purchased item across all transactions
        public string MostPurchasedItem { get; set; } = string.Empty;
        public int MostPurchasedItemCount { get; set; }

        // Total gold across all transactions
        public long TotalGoldSpent { get; set; }

        // Top 5 spenders
        public List<TopSpenderRow> TopSpenders { get; set; } = new List<TopSpenderRow>();
    }

    public class TopSpenderRow
    {
        public string UserName { get; set; } = string.Empty;
        public long TotalSpent { get; set; }
    }
}
