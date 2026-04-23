using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

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

            existingCharacter.CharacterName = updated.CharacterName;
            existingCharacter.UserName = updated.UserName;
            existingCharacter.CurrentLevel = updated.CurrentLevel;
            existingCharacter.Allies = updated.Allies;
            existingCharacter.Equipment = updated.Equipment;
            existingCharacter.CurrentMissions = updated.CurrentMissions;
            existingCharacter.CompletedMissions = updated.CompletedMissions;
            existingCharacter.XP = updated.XP;
            existingCharacter.KnownSpells = updated.KnownSpells;
            existingCharacter.GuildMemberships = updated.GuildMemberships;

            if (!string.Equals(oldCharacterName, existingCharacter.CharacterName, StringComparison.Ordinal))
            {
                foreach (var guild in FakeGameData.Guilds)
                {
                    for (int i = 0; i < guild.Members.Count; i++)
                    {
                        if (guild.Members[i] == oldCharacterName)
                        {
                            guild.Members[i] = existingCharacter.CharacterName;
                        }
                    }
                }
            }

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

            FakeGameData.Characters.Remove(character);
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
            form.PlayerOptions = FakeGameData.Players
                .Select(p => new SelectListItem { Value = p.PlayerName, Text = $"{p.PlayerName} ({p.UserName})" })
                .ToList();

            form.AllyOptions = FakeGameData.Characters
                .Where(c => !currentCharacterId.HasValue || c.CharacterId != currentCharacterId.Value)
                .Select(c => new SelectListItem { Value = c.CharacterName, Text = c.CharacterName })
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

            form.CurrentMissionOptions = FakeGameData.Missions
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.CompletedMissionOptions = FakeGameData.Missions
                .Select(m => new SelectListItem { Value = m.MissionName, Text = m.MissionName })
                .ToList();

            form.SpellOptions = FakeGameData.Spells
                .Select(s => new SelectListItem { Value = s.SpellName, Text = s.SpellName })
                .ToList();

            form.GuildOptions = FakeGameData.Guilds
                .Select(g => new SelectListItem { Value = g.GuildName, Text = g.GuildName })
                .ToList();
        }

        private void SyncPlayerCharacterNames(string playerName)
        {
            Player? player = FakeGameData.Players.FirstOrDefault(p => p.PlayerName == playerName);

            if (player != null)
            {
                player.Characters = FakeGameData.Characters
                    .Where(c => c.UserName == playerName)
                    .Select(c => c.CharacterName)
                    .ToList();
            }
        }

        private void SyncGuildMemberships(PlayerCharacter character)
        {
            foreach (var guild in FakeGameData.Guilds)
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
            }
        }

        private void RemoveCharacterFromGuilds(string characterName, IEnumerable<string> guildNames)
        {
            foreach (var guildName in guildNames)
            {
                Guild? guild = FakeGameData.Guilds.FirstOrDefault(g => g.GuildName == guildName);

                if (guild != null)
                {
                    guild.Members.Remove(characterName);
                    guild.MemberCount = guild.Members.Count;
                }
            }
        }
    }
}
