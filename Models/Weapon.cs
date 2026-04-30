using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MMOngo.Models
{
    public class Weapon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }
        public int WeaponId { get; set; }

        [Required]
        public string WeaponName { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }
        public int Damage { get; set; }

        public List<string> SpecialAttributes { get; set; } = new List<string>();
    }
}