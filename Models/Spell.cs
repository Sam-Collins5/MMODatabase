using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Spell
    {
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