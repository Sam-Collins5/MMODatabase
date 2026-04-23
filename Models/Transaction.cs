using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        public string ItemType { get; set; } = string.Empty;

        public int ItemId { get; set; }

        public int Total { get; set; }

        public string TransactionDate { get; set; } = string.Empty;
    }
}