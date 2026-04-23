using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class ToolItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }
        public int ToolId { get; set; }

        [Required]
        public string ToolName { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }

        public List<string> Usage { get; set; } = new List<string>();
    }
}