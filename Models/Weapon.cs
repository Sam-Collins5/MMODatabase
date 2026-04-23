using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Weapon
    {
        public int WeaponId { get; set; }

        [Required]
        public string WeaponName { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }
        public int Damage { get; set; }

        public List<string> SpecialAttributes { get; set; } = new List<string>();
    }
}