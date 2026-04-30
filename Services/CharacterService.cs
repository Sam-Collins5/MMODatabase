using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.Services;
using MMOngo.ViewModels;
using MongoDB.Driver;
using System.Numerics;

namespace MMOngo.Services
{
    public class CharacterService : ICharacterService
    {
        public List<PlayerCharacter> GetAllCharacters()
        {
            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var filter = Builders<PlayerCharacter>.Filter.Empty;
            return coll.Find(filter).ToList();
        }

        public PlayerCharacter? GetCharacterById(int id)
        {
            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");

            var filter = Builders<PlayerCharacter>.Filter.Empty;
            return coll.Find(filter).ToList().FirstOrDefault(p => p.CharacterId == id);
        }

        public CharacterDetailsViewModel? GetCharacterDetails(int id)
        {
            PlayerCharacter? character = GetCharacterById(id);

            if (character == null)
            {
                return null;
            }

            var weaponColl = MongoConnection.Database.GetCollection<Weapon>("Weapons");
            var weaponFilter = Builders<Weapon>.Filter.Empty;

            var armorColl = MongoConnection.Database.GetCollection<Armor>("Armors");
            var armorFilter = Builders<Armor>.Filter.Empty;

            var toolColl = MongoConnection.Database.GetCollection<ToolItem>("Tools");
            var toolFilter = Builders<ToolItem>.Filter.Empty;

            var spellColl = MongoConnection.Database.GetCollection<Spell>("Spells");
            var spellFilter = Builders<Spell>.Filter.Empty;

            var missionColl = MongoConnection.Database.GetCollection<Mission>("Missions");
            var missionFilter = Builders<Mission>.Filter.Empty;

            var guildColl = MongoConnection.Database.GetCollection<Guild>("Guilds");
            var guildFilter = Builders<Guild>.Filter.Empty;

            return new CharacterDetailsViewModel
            {
                Character = character,
                Weapons = weaponColl.Find(weaponFilter).ToList()
                    .Where(w => character.Equipment.Weapons.Contains(w.WeaponName))
                    .ToList(),
                Armors = armorColl.Find(armorFilter).ToList()
                    .Where(a => character.Equipment.Armor.Contains(a.ArmorName))
                    .ToList(),
                Tools = toolColl.Find(toolFilter).ToList()
                    .Where(t => character.Equipment.Tools.Contains(t.ToolName))
                    .ToList(),
                Spells = spellColl.Find(spellFilter).ToList()
                    .Where(s => character.KnownSpells.Contains(s.SpellName))
                    .ToList(),
                CurrentMissionDetails = missionColl.Find(missionFilter).ToList()
                    .Where(m => character.CurrentMissions.Contains(m.MissionName))
                    .ToList(),
                CompletedMissionDetails = missionColl.Find(missionFilter).ToList()
                    .Where(m => character.CompletedMissions.Contains(m.MissionName))
                    .ToList(),
                GuildDetails = guildColl.Find(guildFilter).ToList()
                    .Where(g => character.GuildMemberships.Contains(g.GuildName))
                    .ToList()
            };
        }

        public CharacterFormViewModel GetCharacterCreateForm()
        {
            CharacterFormViewModel form = new CharacterFormViewModel
            {
                CurrentLevel = 1,
                XP = 0
            };

            PopulateOptions(form, null);
            return form;
        }

        public CharacterFormViewModel? GetCharacterEditForm(int id)
        {
            PlayerCharacter? character = GetCharacterById(id);

            if (character == null)
            {
                return null;
            }

            CharacterFormViewModel form = new CharacterFormViewModel
            {
                CharacterId = character.CharacterId,
                CharacterName = character.CharacterName,
                PlayerName = character.UserName,
                CurrentLevel = character.CurrentLevel,
                XP = character.XP,
                SelectedAllies = new List<string>(character.Allies),
                SelectedWeapons = new List<string>(character.Equipment.Weapons),
                SelectedArmor = new List<string>(character.Equipment.Armor),
                SelectedTools = new List<string>(character.Equipment.Tools),
                SelectedCurrentMissions = new List<string>(character.CurrentMissions),
                SelectedCompletedMissions = new List<string>(character.CompletedMissions),
                SelectedKnownSpells = new List<string>(character.KnownSpells),
                SelectedGuildMemberships = new List<string>(character.GuildMemberships)
            };

            PopulateOptions(form, character.CharacterId);
            return form;
        }

        public void AddCharacter(CharacterFormViewModel form)
        {
            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var filter = Builders<PlayerCharacter>.Filter.Empty;
            var characters = coll.Find(filter).ToList();

            int nextId = characters.Any()
                ? characters.Max(c => c.CharacterId) + 1
                : 1;

            PlayerCharacter character = BuildCharacterFromForm(form, nextId);
            coll.InsertOne(character);
            SyncPlayerCharacterNames(character.UserName);
            SyncGuildMemberships(character);
        }

        public void UpdateCharacter(CharacterFormViewModel form)
        {
            PlayerCharacter? existingCharacter = GetCharacterById(form.CharacterId);

            if (existingCharacter == null)
            {
                return;
            }

            string oldPlayerName = existingCharacter.UserName;
            List<string> oldGuilds = new List<string>(existingCharacter.GuildMemberships);
            string oldCharacterName = existingCharacter.CharacterName;

            PlayerCharacter updated = BuildCharacterFromForm(form, form.CharacterId);

            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var filter = Builders<PlayerCharacter>.Filter.Eq("UserName", existingCharacter.UserName);
            var combinedUpdate = Builders<PlayerCharacter>.Update.Combine(
                Builders<PlayerCharacter>.Update.Set("CharacterName", updated.CharacterName),
                Builders<PlayerCharacter>.Update.Set("UserName", updated.UserName),
                Builders<PlayerCharacter>.Update.Set("CurrentLevel", updated.CurrentLevel),
                Builders<PlayerCharacter>.Update.Set("Allies", updated.Allies),
                Builders<PlayerCharacter>.Update.Set("Equipment", updated.Equipment),
                Builders<PlayerCharacter>.Update.Set("CurrentMissions", updated.CurrentMissions),
                Builders<PlayerCharacter>.Update.Set("CompletedMissions", updated.CompletedMissions),
                Builders<PlayerCharacter>.Update.Set("XP", updated.XP),
                Builders<PlayerCharacter>.Update.Set("KnownSpells", updated.KnownSpells),
                Builders<PlayerCharacter>.Update.Set("GuildMemberships", updated.GuildMemberships)
            );
            coll.UpdateOne(filter, combinedUpdate);

            var guildColl = MongoConnection.Database.GetCollection<Guild>("Guilds");
            var guildFilter = Builders<Guild>.Filter.Empty;
            var guilds = guildColl.Find(guildFilter).ToList();

            //if (!string.Equals(oldCharacterName, existingCharacter.CharacterName, StringComparison.Ordinal))
            //{
            //    foreach (var guild in guilds)
            //    {
            //        for (int i = 0; i < guild.Members.Count; i++)
            //        {
            //            if (guild.Members[i] == oldCharacterName)
            //            {
            //                guild.Members[i] = existingCharacter.CharacterName;
            //            }
            //        }
            //    }
            //}

            SyncPlayerCharacterNames(oldPlayerName);
            SyncPlayerCharacterNames(existingCharacter.UserName);
            RemoveCharacterFromGuilds(existingCharacter.CharacterName, oldGuilds.Except(existingCharacter.GuildMemberships));
            SyncGuildMemberships(existingCharacter);
        }

        public void UpdateCharacter(PlayerCharacter form)
        {
            PlayerCharacter? existingCharacter = GetCharacterById(form.CharacterId);

            if (existingCharacter == null)
            {
                return;
            }

            string oldPlayerName = existingCharacter.UserName;
            List<string> oldGuilds = new List<string>(existingCharacter.GuildMemberships);
            string oldCharacterName = existingCharacter.CharacterName;

            PlayerCharacter updated = form;

            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var filter = Builders<PlayerCharacter>.Filter.Eq("UserName", existingCharacter.UserName);
            var combinedUpdate = Builders<PlayerCharacter>.Update.Combine(
                Builders<PlayerCharacter>.Update.Set("CharacterName", updated.CharacterName),
                Builders<PlayerCharacter>.Update.Set("UserName", updated.UserName),
                Builders<PlayerCharacter>.Update.Set("CurrentLevel", updated.CurrentLevel),
                Builders<PlayerCharacter>.Update.Set("Allies", updated.Allies),
                Builders<PlayerCharacter>.Update.Set("Equipment", updated.Equipment),
                Builders<PlayerCharacter>.Update.Set("CurrentMissions", updated.CurrentMissions),
                Builders<PlayerCharacter>.Update.Set("CompletedMissions", updated.CompletedMissions),
                Builders<PlayerCharacter>.Update.Set("XP", updated.XP),
                Builders<PlayerCharacter>.Update.Set("KnownSpells", updated.KnownSpells),
                Builders<PlayerCharacter>.Update.Set("GuildMemberships", updated.GuildMemberships)
            );
            coll.UpdateOne(filter, combinedUpdate);

            var guildColl = MongoConnection.Database.GetCollection<Guild>("Guilds");
            var guildFilter = Builders<Guild>.Filter.Empty;
            var guilds = guildColl.Find(guildFilter).ToList();

            SyncPlayerCharacterNames(oldPlayerName);
            SyncPlayerCharacterNames(existingCharacter.UserName);
            RemoveCharacterFromGuilds(existingCharacter.CharacterName, oldGuilds.Except(existingCharacter.GuildMemberships));
            SyncGuildMemberships(existingCharacter);
        }

        public void DeleteCharacter(int id)
        {
            PlayerCharacter? character = GetCharacterById(id);

            if (character == null)
            {
                return;
            }

            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var filter = Builders<PlayerCharacter>.Filter.Eq("CharacterId", id);
            coll.DeleteOne(filter);

            SyncPlayerCharacterNames(character.UserName);
            RemoveCharacterFromGuilds(character.CharacterName, character.GuildMemberships);
        }

        private PlayerCharacter BuildCharacterFromForm(CharacterFormViewModel form, int characterId)
        {
            return new PlayerCharacter
            {
                CharacterId = characterId,
                CharacterName = form.CharacterName.Trim(),
                UserName = form.PlayerName,
                CurrentLevel = form.CurrentLevel,
                XP = form.XP,
                Allies = CleanList(form.SelectedAllies),
                Equipment = new EquipmentSet
                {
                    Weapons = CleanList(form.SelectedWeapons),
                    Armor = CleanList(form.SelectedArmor),
                    Tools = CleanList(form.SelectedTools)
                },
                CurrentMissions = CleanList(form.SelectedCurrentMissions),
                CompletedMissions = CleanList(form.SelectedCompletedMissions),
                KnownSpells = CleanList(form.SelectedKnownSpells),
                GuildMemberships = CleanList(form.SelectedGuildMemberships)
            };
        }

        private List<string> CleanList(List<string>? values)
        {
            return values?
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct()
                .ToList() ?? new List<string>();
        }

        private void PopulateOptions(CharacterFormViewModel form, int? currentCharacterId)
        {
            form.PlayerOptions = new PlayerService().GetAllPlayers()
                .Select(p => new SelectListItem { Value = p.PlayerName, Text = $"{p.PlayerName} ({p.UserName})" })
                .ToList();

            form.AllyOptions = GetAllCharacters()
                .Where(c => !currentCharacterId.HasValue || c.CharacterId != currentCharacterId.Value)
                .Select(c => new SelectListItem { Value = c.CharacterName, Text = c.CharacterName })
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

            form.CurrentMissionOptions = MongoConnection.Database.GetCollection<Mission>("Missions").Find(Builders<Mission>.Filter.Empty).ToList()
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.CompletedMissionOptions = MongoConnection.Database.GetCollection<Mission>("Missions").Find(Builders<Mission>.Filter.Empty).ToList()
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.SpellOptions = MongoConnection.Database.GetCollection<Spell>("Spells").Find(Builders<Spell>.Filter.Empty).ToList()
                .Select(s => new SelectListItem { Value = s.SpellName, Text = s.SpellName })
                .ToList();

            form.GuildOptions = MongoConnection.Database.GetCollection<Guild>("Guilds").Find(Builders<Guild>.Filter.Empty).ToList()
                .Select(g => new SelectListItem { Value = g.GuildName, Text = g.GuildName })
                .ToList();
        }

        private void SyncPlayerCharacterNames(string playerName)
        {
            var playerService = new PlayerService();
            Player? player = playerService.GetAllPlayers().FirstOrDefault(p => p.PlayerName == playerName);

            if (player != null)
            {
                player.Characters = GetAllCharacters()
                    .Where(c => c.UserName == playerName)
                    .Select(c => c.CharacterName)
                    .ToList();
            }
        }

        private void SyncGuildMemberships(PlayerCharacter character)
        {
            var guildService = new GuildService();
            foreach (var guild in guildService.GetAllGuilds())
            {
                bool shouldBeMember = character.GuildMemberships.Contains(guild.GuildName);
                bool alreadyMember = guild.Members.Contains(character.CharacterName);

                if (shouldBeMember && !alreadyMember)
                {
                    guild.Members.Add(character.CharacterName);
                }

                if (!shouldBeMember && alreadyMember)
                {
                    guild.Members.Remove(character.CharacterName);
                }

                guild.MemberCount = guild.Members.Count;
                guildService.UpdateGuild(guild);
            }
        }

        private void RemoveCharacterFromGuilds(string characterName, IEnumerable<string> guildNames)
        {
            var guildService = new GuildService();
            foreach (var guildName in guildNames)
            {
                Guild? guild = guildService.GetAllGuilds().FirstOrDefault(g => g.GuildName == guildName);

                if (guild != null)
                {
                    guild.Members.Remove(characterName);
                    guild.MemberCount = guild.Members.Count;
                    guildService.UpdateGuild(guild);
                }
            }
        }
    }
}
