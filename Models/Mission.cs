using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Mission
    {
        [Required]
        public string MissionName { get; set; } = string.Empty;

        [Required]
        public string MissionType { get; set; } = string.Empty;

        public string QuestGiver { get; set; } = string.Empty;

        public int Reward { get; set; }
    }
}