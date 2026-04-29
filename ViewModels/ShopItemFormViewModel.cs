using System.ComponentModel.DataAnnotations;

namespace MMOngo.ViewModels
{
    public class ShopItemFormViewModel
    {
        [Required]
        public string Category { get; set; } = string.Empty;

        public int ItemId { get; set; }

        public string OriginalName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }

        [Display(Name = "Spell Damage")]
        public int SpellDamage { get; set; }

        [Display(Name = "Spell Healing")]
        public int SpellHealing { get; set; }

        [Display(Name = "MP Cost")]
        public int MpCost { get; set; }

        [Display(Name = "Gold Cost")]
        public int GoldCost { get; set; }

        public int Range { get; set; }

        [Display(Name = "Special Attributes")]
        public string SpecialAttributesText { get; set; } = string.Empty;

        [Display(Name = "Usage")]
        public string UsageText { get; set; } = string.Empty;

        public bool IsWeapon => Category == "weapon";
        public bool IsArmor => Category == "armor";
        public bool IsTool => Category == "tool";
        public bool IsSpell => Category == "spell";

        public string CategoryTitle => Category switch
        {
            "weapon" => "Weapon",
            "armor" => "Armor",
            "tool" => "Tool",
            "spell" => "Spell",
            _ => "Shop Item"
        };
    }
}
