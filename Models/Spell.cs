using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class Spell
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }
        public int SpellId { get; set; }

        [Required]
        public string SpellName { get; set; } = string.Empty;

        public int SpellDamage { get; set; }
        public int SpellHealing { get; set; }
        public int MpCost { get; set; }
        public int GoldCost { get; set; }
        public int Range { get; set; }
    }
}