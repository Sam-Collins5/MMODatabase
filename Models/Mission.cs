using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class Mission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string MissionName { get; set; } = string.Empty;

        [Required]
        public string MissionType { get; set; } = string.Empty;

        public string QuestGiver { get; set; } = string.Empty;

        public int Reward { get; set; }
    }
}