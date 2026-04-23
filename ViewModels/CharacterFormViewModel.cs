using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MMOngo.ViewModels
{
    public class CharacterFormViewModel
    {
        public int CharacterId { get; set; }

        [Required]
        public string CharacterName { get; set; } = string.Empty;

        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Range(1, 999)]
        public int CurrentLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int XP { get; set; }

        public List<string> SelectedAllies { get; set; } = new();
        public List<string> SelectedWeapons { get; set; } = new();
        public List<string> SelectedArmor { get; set; } = new();
        public List<string> SelectedTools { get; set; } = new();
        public List<string> SelectedCurrentMissions { get; set; } = new();
        public List<string> SelectedCompletedMissions { get; set; } = new();
        public List<string> SelectedKnownSpells { get; set; } = new();
        public List<string> SelectedGuildMemberships { get; set; } = new();

        public List<SelectListItem> PlayerOptions { get; set; } = new();
        public List<SelectListItem> AllyOptions { get; set; } = new();
        public List<SelectListItem> WeaponOptions { get; set; } = new();
        public List<SelectListItem> ArmorOptions { get; set; } = new();
        public List<SelectListItem> ToolOptions { get; set; } = new();
        public List<SelectListItem> CurrentMissionOptions { get; set; } = new();
        public List<SelectListItem> CompletedMissionOptions { get; set; } = new();
        public List<SelectListItem> SpellOptions { get; set; } = new();
        public List<SelectListItem> GuildOptions { get; set; } = new();
    }
}
