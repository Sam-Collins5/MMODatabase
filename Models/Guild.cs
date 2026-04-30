using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Guild
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; }

        [Required]
        public string GuildName { get; set; } = string.Empty;

        [Required]
        public string Creator { get; set; } = string.Empty;

        public int MemberCount { get; set; }

        public List<string> Members { get; set; } = new List<string>();

        public string CreationDate { get; set; } = string.Empty;

        public List<string> Benefits { get; set; } = new List<string>();
    }
}