using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class ShopService : IShopService
    {
        public ShopViewModel GetShopData(string? weaponName, string? armorName, string? spellName, string? toolName)
        {
            var weaponColl = MongoConnection.Database.GetCollection<Weapon>("Weapons");
            var weaponFilter = Builders<Weapon>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(weaponName))
            {
                weaponFilter = Builders<Weapon>.Filter.Regex(w => w.WeaponName,
                    new MongoDB.Bson.BsonRegularExpression(weaponName, "i"));
            }

            var armorColl = MongoConnection.Database.GetCollection<Armor>("Armors");
            var armorFilter = Builders<Armor>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(armorName))
            {
                armorFilter = Builders<Armor>.Filter.Regex(a => a.ArmorName,
                    new MongoDB.Bson.BsonRegularExpression(armorName, "i"));
            }

            var toolColl = MongoConnection.Database.GetCollection<ToolItem>("Tools");
            var toolFilter = Builders<ToolItem>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(toolName))
            {
                toolFilter = Builders<ToolItem>.Filter.Regex(t => t.ToolName,
                    new MongoDB.Bson.BsonRegularExpression(toolName, "i"));
            }

            var spellColl = MongoConnection.Database.GetCollection<Spell>("Spells");
            var spellFilter = Builders<Spell>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(spellName))
            {
                spellFilter = Builders<Spell>.Filter.Regex(s => s.SpellName,
                    new MongoDB.Bson.BsonRegularExpression(spellName, "i"));
            }

            return new ShopViewModel
            {
                Weapons = weaponColl.Find(weaponFilter).ToList(),
                Armors = armorColl.Find(armorFilter).ToList(),
                Tools = toolColl.Find(toolFilter).ToList(),
                Spells = spellColl.Find(spellFilter).ToList(),

            };
        }

        public ShopItemFormViewModel GetItemCreateForm(string category)
        {
            return new ShopItemFormViewModel { Category = NormalizeCategory(category) };
        }

        public ShopItemFormViewModel? GetItemEditForm(string category, int id)
        {
            string normalized = NormalizeCategory(category);

            return normalized switch
            {
                "weapon" => FakeGameData.Weapons.Where(w => w.WeaponId == id).Select(w => new ShopItemFormViewModel
                {
                    Category = normalized,
                    ItemId = w.WeaponId,
                    OriginalName = w.WeaponName,
                    Name = w.WeaponName,
                    Weight = w.Weight,
                    Price = w.Price,
                    Damage = w.Damage,
                    SpecialAttributesText = string.Join(", ", w.SpecialAttributes)
                }).FirstOrDefault(),
                "armor" => FakeGameData.Armors.Where(a => a.ArmorId == id).Select(a => new ShopItemFormViewModel
                {
                    Category = normalized,
                    ItemId = a.ArmorId,
                    OriginalName = a.ArmorName,
                    Name = a.ArmorName,
                    Weight = a.Weight,
                    Price = a.Price,
                    Defense = a.Defense,
                    SpecialAttributesText = string.Join(", ", a.SpecialAttributes)
                }).FirstOrDefault(),
                "tool" => FakeGameData.Tools.Where(t => t.ToolId == id).Select(t => new ShopItemFormViewModel
                {
                    Category = normalized,
                    ItemId = t.ToolId,
                    OriginalName = t.ToolName,
                    Name = t.ToolName,
                    Weight = t.Weight,
                    Price = t.Price,
                    UsageText = string.Join(", ", t.Usage)
                }).FirstOrDefault(),
                "spell" => FakeGameData.Spells.Where(s => s.SpellId == id).Select(s => new ShopItemFormViewModel
                {
                    Category = normalized,
                    ItemId = s.SpellId,
                    OriginalName = s.SpellName,
                    Name = s.SpellName,
                    SpellDamage = s.SpellDamage,
                    SpellHealing = s.SpellHealing,
                    MpCost = s.MpCost,
                    GoldCost = s.GoldCost,
                    Range = s.Range
                }).FirstOrDefault(),
                _ => null
            };
        }

        public object? GetItemForDelete(string category, int id)
        {
            string normalized = NormalizeCategory(category);

            return normalized switch
            {
                "weapon" => FakeGameData.Weapons.FirstOrDefault(w => w.WeaponId == id),
                "armor" => FakeGameData.Armors.FirstOrDefault(a => a.ArmorId == id),
                "tool" => FakeGameData.Tools.FirstOrDefault(t => t.ToolId == id),
                "spell" => FakeGameData.Spells.FirstOrDefault(s => s.SpellId == id),
                _ => null
            };
        }

        public void AddItem(ShopItemFormViewModel form)
        {
            string category = NormalizeCategory(form.Category);

            switch (category)
            {
                case "weapon":
                    FakeGameData.Weapons.Add(new Weapon
                    {
                        WeaponId = NextId(FakeGameData.Weapons.Select(w => w.WeaponId)),
                        WeaponName = form.Name.Trim(),
                        Weight = form.Weight,
                        Price = form.Price,
                        Damage = form.Damage,
                        SpecialAttributes = SplitList(form.SpecialAttributesText)
                    });
                    break;
                case "armor":
                    FakeGameData.Armors.Add(new Armor
                    {
                        ArmorId = NextId(FakeGameData.Armors.Select(a => a.ArmorId)),
                        ArmorName = form.Name.Trim(),
                        Weight = form.Weight,
                        Price = form.Price,
                        Defense = form.Defense,
                        SpecialAttributes = SplitList(form.SpecialAttributesText)
                    });
                    break;
                case "tool":
                    FakeGameData.Tools.Add(new ToolItem
                    {
                        ToolId = NextId(FakeGameData.Tools.Select(t => t.ToolId)),
                        ToolName = form.Name.Trim(),
                        Weight = form.Weight,
                        Price = form.Price,
                        Usage = SplitList(form.UsageText)
                    });
                    break;
                case "spell":
                    FakeGameData.Spells.Add(new Spell
                    {
                        SpellId = NextId(FakeGameData.Spells.Select(s => s.SpellId)),
                        SpellName = form.Name.Trim(),
                        SpellDamage = form.SpellDamage,
                        SpellHealing = form.SpellHealing,
                        MpCost = form.MpCost,
                        GoldCost = form.GoldCost,
                        Range = form.Range
                    });
                    break;
            }
        }

        public void UpdateItem(ShopItemFormViewModel form)
        {
            string category = NormalizeCategory(form.Category);
            string oldName = form.OriginalName;
            string newName = form.Name.Trim();

            switch (category)
            {
                case "weapon":
                    Weapon? weapon = FakeGameData.Weapons.FirstOrDefault(w => w.WeaponId == form.ItemId);
                    if (weapon == null) return;
                    weapon.WeaponName = newName;
                    weapon.Weight = form.Weight;
                    weapon.Price = form.Price;
                    weapon.Damage = form.Damage;
                    weapon.SpecialAttributes = SplitList(form.SpecialAttributesText);
                    RenameWeaponReferences(oldName, newName);
                    break;
                case "armor":
                    Armor? armor = FakeGameData.Armors.FirstOrDefault(a => a.ArmorId == form.ItemId);
                    if (armor == null) return;
                    armor.ArmorName = newName;
                    armor.Weight = form.Weight;
                    armor.Price = form.Price;
                    armor.Defense = form.Defense;
                    armor.SpecialAttributes = SplitList(form.SpecialAttributesText);
                    RenameArmorReferences(oldName, newName);
                    break;
                case "tool":
                    ToolItem? tool = FakeGameData.Tools.FirstOrDefault(t => t.ToolId == form.ItemId);
                    if (tool == null) return;
                    tool.ToolName = newName;
                    tool.Weight = form.Weight;
                    tool.Price = form.Price;
                    tool.Usage = SplitList(form.UsageText);
                    RenameToolReferences(oldName, newName);
                    break;
                case "spell":
                    Spell? spell = FakeGameData.Spells.FirstOrDefault(s => s.SpellId == form.ItemId);
                    if (spell == null) return;
                    spell.SpellName = newName;
                    spell.SpellDamage = form.SpellDamage;
                    spell.SpellHealing = form.SpellHealing;
                    spell.MpCost = form.MpCost;
                    spell.GoldCost = form.GoldCost;
                    spell.Range = form.Range;
                    RenameSpellReferences(oldName, newName);
                    break;
            }
        }

        public void DeleteItem(string category, int id)
        {
            string normalized = NormalizeCategory(category);

            switch (normalized)
            {
                case "weapon":
                    Weapon? weapon = FakeGameData.Weapons.FirstOrDefault(w => w.WeaponId == id);
                    if (weapon == null) return;
                    RemoveWeaponReferences(weapon.WeaponName);
                    FakeGameData.Weapons.Remove(weapon);
                    RemoveTransactions("Weapon", weapon.WeaponId);
                    break;
                case "armor":
                    Armor? armor = FakeGameData.Armors.FirstOrDefault(a => a.ArmorId == id);
                    if (armor == null) return;
                    RemoveArmorReferences(armor.ArmorName);
                    FakeGameData.Armors.Remove(armor);
                    RemoveTransactions("Armor", armor.ArmorId);
                    break;
                case "tool":
                    ToolItem? tool = FakeGameData.Tools.FirstOrDefault(t => t.ToolId == id);
                    if (tool == null) return;
                    RemoveToolReferences(tool.ToolName);
                    FakeGameData.Tools.Remove(tool);
                    RemoveTransactions("Tool", tool.ToolId);
                    break;
                case "spell":
                    Spell? spell = FakeGameData.Spells.FirstOrDefault(s => s.SpellId == id);
                    if (spell == null) return;
                    RemoveSpellReferences(spell.SpellName);
                    FakeGameData.Spells.Remove(spell);
                    RemoveTransactions("Spell", spell.SpellId);
                    break;
            }
        }

        private string NormalizeCategory(string category)
        {
            return (category ?? string.Empty).Trim().ToLowerInvariant();
        }

        private int NextId(IEnumerable<int> ids)
        {
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        private List<string> SplitList(string value)
        {
            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
        }

        private void RenameWeaponReferences(string oldName, string newName)
        {
            RenameEquipmentReference(oldName, newName, e => e.Weapons);
            RenameNpcWares(oldName, newName);
        }

        private void RenameArmorReferences(string oldName, string newName)
        {
            RenameEquipmentReference(oldName, newName, e => e.Armor);
            RenameNpcWares(oldName, newName);
        }

        private void RenameToolReferences(string oldName, string newName)
        {
            RenameEquipmentReference(oldName, newName, e => e.Tools);
            RenameNpcWares(oldName, newName);
        }

        private void RenameSpellReferences(string oldName, string newName)
        {
            foreach (var character in FakeGameData.Characters)
            {
                ReplaceName(character.KnownSpells, oldName, newName);
            }

            foreach (var npc in FakeGameData.Npcs)
            {
                ReplaceName(npc.KnownSpells, oldName, newName);
                ReplaceName(npc.WaresSold, oldName, newName);
            }
        }

        private void RemoveWeaponReferences(string name)
        {
            RemoveEquipmentReference(name, e => e.Weapons);
            RemoveNpcWares(name);
        }

        private void RemoveArmorReferences(string name)
        {
            RemoveEquipmentReference(name, e => e.Armor);
            RemoveNpcWares(name);
        }

        private void RemoveToolReferences(string name)
        {
            RemoveEquipmentReference(name, e => e.Tools);
            RemoveNpcWares(name);
        }

        private void RemoveSpellReferences(string name)
        {
            foreach (var character in FakeGameData.Characters)
            {
                character.KnownSpells.RemoveAll(s => s == name);
            }

            foreach (var npc in FakeGameData.Npcs)
            {
                npc.KnownSpells.RemoveAll(s => s == name);
                npc.WaresSold.RemoveAll(w => w == name);
            }
        }

        private void RenameEquipmentReference(string oldName, string newName, Func<EquipmentSet, List<string>> selector)
        {
            foreach (var character in FakeGameData.Characters)
            {
                ReplaceName(selector(character.Equipment), oldName, newName);
            }

            foreach (var npc in FakeGameData.Npcs)
            {
                ReplaceName(selector(npc.Equipment), oldName, newName);
            }
        }

        private void RemoveEquipmentReference(string name, Func<EquipmentSet, List<string>> selector)
        {
            foreach (var character in FakeGameData.Characters)
            {
                selector(character.Equipment).RemoveAll(i => i == name);
            }

            foreach (var npc in FakeGameData.Npcs)
            {
                selector(npc.Equipment).RemoveAll(i => i == name);
            }
        }

        private void RenameNpcWares(string oldName, string newName)
        {
            foreach (var npc in FakeGameData.Npcs)
            {
                ReplaceName(npc.WaresSold, oldName, newName);
            }
        }

        private void RemoveNpcWares(string name)
        {
            foreach (var npc in FakeGameData.Npcs)
            {
                npc.WaresSold.RemoveAll(w => w == name);
            }
        }

        private void ReplaceName(List<string> values, string oldName, string newName)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == oldName)
                {
                    values[i] = newName;
                }
            }
        }

        private void RemoveTransactions(string itemType, int itemId)
        {
            FakeGameData.Transactions.RemoveAll(t => t.ItemType == itemType && t.ItemId == itemId);
        }
    }
}
