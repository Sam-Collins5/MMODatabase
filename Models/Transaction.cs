using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        public int TransactionId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string ItemType { get; set; } = string.Empty;

        public int ItemId { get; set; }

        public int Total { get; set; }

        public string TransactionDate { get; set; } = string.Empty;
    }
}