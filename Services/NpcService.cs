using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Services
{
    public class NpcService : INpcService
    {
        public List<Npc> GetAllNpcs()
        {
            return FakeGameData.Npcs;
        }

        public Npc? GetNpcByName(string name)
        {
            return FakeGameData.Npcs.FirstOrDefault(n => n.NpcName == name);
        }

        public NpcFormViewModel GetNpcCreateForm()
        {
            NpcFormViewModel form = new NpcFormViewModel();
            PopulateOptions(form);
            return form;
        }

        public NpcFormViewModel? GetNpcEditForm(string name)
        {
            Npc? npc = GetNpcByName(name);

            if (npc == null)
            {
                return null;
            }

            NpcFormViewModel form = new NpcFormViewModel
            {
                OriginalNpcName = npc.NpcName,
                NpcName = npc.NpcName,
                LocationsText = string.Join(", ", npc.Locations),
                SelectedWaresSold = new List<string>(npc.WaresSold),
                SelectedQuests = new List<string>(npc.Quests),
                SelectedWeapons = new List<string>(npc.Equipment.Weapons),
                SelectedArmor = new List<string>(npc.Equipment.Armor),
                SelectedTools = new List<string>(npc.Equipment.Tools),
                XP = npc.XP,
                SelectedKnownSpells = new List<string>(npc.KnownSpells)
            };

            PopulateOptions(form);
            return form;
        }

        public void AddNpc(NpcFormViewModel form)
        {
            FakeGameData.Npcs.Add(BuildNpc(form));
        }

        public void UpdateNpc(NpcFormViewModel form)
        {
            Npc? npc = GetNpcByName(form.OriginalNpcName);

            if (npc == null)
            {
                return;
            }

            string oldName = npc.NpcName;
            Npc updated = BuildNpc(form);

            npc.NpcName = updated.NpcName;
            npc.Locations = updated.Locations;
            npc.WaresSold = updated.WaresSold;
            npc.Quests = updated.Quests;
            npc.Equipment = updated.Equipment;
            npc.XP = updated.XP;
            npc.KnownSpells = updated.KnownSpells;

            if (!string.Equals(oldName, npc.NpcName, StringComparison.Ordinal))
            {
                foreach (var mission in FakeGameData.Missions.Where(m => m.QuestGiver == oldName))
                {
                    mission.QuestGiver = npc.NpcName;
                }
            }
        }

        public void DeleteNpc(string name)
        {
            Npc? npc = GetNpcByName(name);

            if (npc == null)
            {
                return;
            }

            foreach (var mission in FakeGameData.Missions.Where(m => m.QuestGiver == npc.NpcName))
            {
                mission.QuestGiver = string.Empty;
            }

            FakeGameData.Npcs.Remove(npc);
        }

        private Npc BuildNpc(NpcFormViewModel form)
        {
            return new Npc
            {
                NpcName = form.NpcName.Trim(),
                Locations = SplitList(form.LocationsText),
                WaresSold = CleanList(form.SelectedWaresSold),
                Quests = CleanList(form.SelectedQuests),
                Equipment = new EquipmentSet
                {
                    Weapons = CleanList(form.SelectedWeapons),
                    Armor = CleanList(form.SelectedArmor),
                    Tools = CleanList(form.SelectedTools)
                },
                XP = form.XP,
                KnownSpells = CleanList(form.SelectedKnownSpells)
            };
        }

        private void PopulateOptions(NpcFormViewModel form)
        {
            form.WareOptions = FakeGameData.Weapons
                .Select(w => new SelectListItem { Value = w.WeaponName, Text = $"Weapon: {w.WeaponName}" })
                .Concat(FakeGameData.Armors.Select(a => new SelectListItem { Value = a.ArmorName, Text = $"Armor: {a.ArmorName}" }))
                .Concat(FakeGameData.Tools.Select(t => new SelectListItem { Value = t.ToolName, Text = $"Tool: {t.ToolName}" }))
                .Concat(FakeGameData.Spells.Select(s => new SelectListItem { Value = s.SpellName, Text = $"Spell: {s.SpellName}" }))
                .ToList();

            form.QuestOptions = FakeGameData.Missions
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.WeaponOptions = FakeGameData.Weapons
                .Select(w => new SelectListItem { Value = w.WeaponName, Text = w.WeaponName })
                .ToList();

            form.ArmorOptions = FakeGameData.Armors
                .Select(a => new SelectListItem { Value = a.ArmorName, Text = a.ArmorName })
                .ToList();

            form.ToolOptions = FakeGameData.Tools
                .Select(t => new SelectListItem { Value = t.ToolName, Text = t.ToolName })
                .ToList();

            form.SpellOptions = FakeGameData.Spells
                .Select(s => new SelectListItem { Value = s.SpellName, Text = s.SpellName })
                .ToList();
        }

        private List<string> SplitList(string value)
        {
            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
        }

        private List<string> CleanList(List<string>? values)
        {
            return values?
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct()
                .ToList() ?? new List<string>();
        }
    }
}
