using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class Npc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string NpcName { get; set; } = string.Empty;

        public List<string> Locations { get; set; } = new List<string>();

        public List<string> WaresSold { get; set; } = new List<string>();

        public List<string> Quests { get; set; } = new List<string>();

        public EquipmentSet Equipment { get; set; } = new EquipmentSet();

        public int XP { get; set; }

        public List<string> KnownSpells { get; set; } = new List<string>();
    }
}