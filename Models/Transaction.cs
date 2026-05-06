using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string ItemType { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int Total { get; set; }

        [Required]
        public string TransactionDate { get; set; } = string.Empty;
    }
}