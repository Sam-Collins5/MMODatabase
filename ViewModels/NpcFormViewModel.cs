using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MMOngo.ViewModels
{
    public class NpcFormViewModel
    {
        public string OriginalNpcName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "NPC Name")]
        public string NpcName { get; set; } = string.Empty;

        [Display(Name = "Locations")]
        public string LocationsText { get; set; } = string.Empty;

        [Display(Name = "Wares Sold")]
        public List<string> SelectedWaresSold { get; set; } = new List<string>();

        [Display(Name = "Quests")]
        public List<string> SelectedQuests { get; set; } = new List<string>();

        [Display(Name = "Equipped Weapons")]
        public List<string> SelectedWeapons { get; set; } = new List<string>();

        [Display(Name = "Equipped Armor")]
        public List<string> SelectedArmor { get; set; } = new List<string>();

        [Display(Name = "Equipped Tools")]
        public List<string> SelectedTools { get; set; } = new List<string>();

        public int XP { get; set; }

        [Display(Name = "Known Spells")]
        public List<string> SelectedKnownSpells { get; set; } = new List<string>();

        public List<SelectListItem> WareOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QuestOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WeaponOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ArmorOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ToolOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SpellOptions { get; set; } = new List<SelectListItem>();
    }
}
