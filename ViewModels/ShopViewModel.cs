using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class ShopViewModel
    {
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public List<Armor> Armors { get; set; } = new List<Armor>();
        public List<ToolItem> Tools { get; set; } = new List<ToolItem>();
        public List<Spell> Spells { get; set; } = new List<Spell>();
    }
}