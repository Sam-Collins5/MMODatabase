using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class PlayerCharacter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        public int CharacterId { get; set; }

        [Required]
        public string CharacterName { get; set; } = string.Empty;

        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Range(1, 999)]
        public int CurrentLevel { get; set; }

        public List<string> Allies { get; set; } = new List<string>();

        public EquipmentSet Equipment { get; set; } = new EquipmentSet();

        public List<string> CurrentMissions { get; set; } = new List<string>();

        public List<string> CompletedMissions { get; set; } = new List<string>();

        public int XP { get; set; }

        public List<string> KnownSpells { get; set; } = new List<string>();

        public List<string> GuildMemberships { get; set; } = new List<string>();
    }
}