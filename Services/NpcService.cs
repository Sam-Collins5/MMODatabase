using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class NpcService : INpcService
    {
        public List<Npc> GetAllNpcs()
        {
            var npcColl = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var npcFilter = Builders<Npc>.Filter.Empty;

            return npcColl.Find(npcFilter).ToList();
        }

        public Npc? GetNpcByName(string name)
        {
            var npcColl = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var npcFilter = Builders<Npc>.Filter.Empty;

            return npcColl.Find(npcFilter).ToList()
                .FirstOrDefault(n => n.NpcName == name);
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
            var coll = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var filter = Builders<Npc>.Filter.Empty;
            var npcs = coll.Find(filter).ToList();

            Npc npc = BuildNpc(form);
            coll.InsertOne(npc);           
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
                foreach (var mission in MongoConnection.Database.GetCollection<Mission>("Missions").Find(Builders<Mission>.Filter.Empty).ToList().Where(m => m.QuestGiver == oldName))
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

            foreach (var mission in MongoConnection.Database.GetCollection<Mission>("Missions").Find(Builders<Mission>.Filter.Empty).ToList().Where(m => m.QuestGiver == npc.NpcName))
            {
                mission.QuestGiver = string.Empty;
            }

            var coll = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var filter = Builders<Npc>.Filter.Eq("NpcName", npc.NpcName);
            coll.DeleteOne(filter);
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
            form.WareOptions = MongoConnection.Database.GetCollection<Weapon>("Weapons").Find(Builders<Weapon>.Filter.Empty).ToList()
                .Select(w => new SelectListItem { Value = w.WeaponName, Text = $"Weapon: {w.WeaponName}" })
                .Concat(MongoConnection.Database.GetCollection<Armor>("Armors").Find(Builders<Armor>.Filter.Empty).ToList().Select(a => new SelectListItem { Value = a.ArmorName, Text = $"Armor: {a.ArmorName}" }))
                .Concat(MongoConnection.Database.GetCollection<ToolItem>("Tools").Find(Builders<ToolItem>.Filter.Empty).ToList().Select(t => new SelectListItem { Value = t.ToolName, Text = $"Tool: {t.ToolName}" }))
                .Concat(MongoConnection.Database.GetCollection<Spell>("Spells").Find(Builders<Spell>.Filter.Empty).ToList().Select(s => new SelectListItem { Value = s.SpellName, Text = $"Spell: {s.SpellName}" }))
                .ToList();

            form.QuestOptions = MongoConnection.Database.GetCollection<Mission>("Missions").Find(Builders<Mission>.Filter.Empty).ToList()
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.WeaponOptions = MongoConnection.Database.GetCollection<Weapon>("Weapons").Find(Builders<Weapon>.Filter.Empty).ToList()
                .Select(w => new SelectListItem { Value = w.WeaponName, Text = w.WeaponName })
                .ToList();

            form.ArmorOptions = MongoConnection.Database.GetCollection<Armor>("Armors").Find(Builders<Armor>.Filter.Empty).ToList()
                .Select(a => new SelectListItem { Value = a.ArmorName, Text = a.ArmorName })
                .ToList();

            form.ToolOptions = MongoConnection.Database.GetCollection<ToolItem>("Tools").Find(Builders<ToolItem>.Filter.Empty).ToList()
                .Select(t => new SelectListItem { Value = t.ToolName, Text = t.ToolName })
                .ToList();

            form.SpellOptions = MongoConnection.Database.GetCollection<Spell>("Spells").Find(Builders<Spell>.Filter.Empty).ToList()
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
