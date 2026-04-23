using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class CharacterDetailsViewModel
    {
        public PlayerCharacter Character { get; set; } = new PlayerCharacter();
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public List<Armor> Armors { get; set; } = new List<Armor>();
        public List<ToolItem> Tools { get; set; } = new List<ToolItem>();
        public List<Spell> Spells { get; set; } = new List<Spell>();
        public List<Mission> CurrentMissionDetails { get; set; } = new List<Mission>();
        public List<Mission> CompletedMissionDetails { get; set; } = new List<Mission>();
        public List<Guild> GuildDetails { get; set; } = new List<Guild>();
    }
}