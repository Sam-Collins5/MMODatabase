using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Npc
    {
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