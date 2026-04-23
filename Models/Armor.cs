using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Armor
    {
        public int ArmorId { get; set; }

        [Required]
        public string ArmorName { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }
        public int Defense { get; set; }

        public List<string> SpecialAttributes { get; set; } = new List<string>();
    }
}