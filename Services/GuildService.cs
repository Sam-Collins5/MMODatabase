using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;
using System.Numerics;
using System.Reflection;

namespace MMOngo.Services
{
    public class GuildService : IGuildService
    {
        public List<Guild> GetAllGuilds()
        {
            var coll = MongoConnection.Database.GetCollection<Guild>("Guilds");

            var filter = Builders<Guild>.Filter.Empty;
            return coll.Find(filter).ToList();
        }

        public Guild? GetGuildByName(string name)
        {
            var coll = MongoConnection.Database.GetCollection<Guild>("Guilds");

            var filter = Builders<Guild>.Filter.Empty;
            return coll.Find(filter).ToList().FirstOrDefault(g => g.GuildName == name);
        }

        public GuildDetailsViewModel? GetGuildDetails(string name)
        {
            Guild? guild = GetGuildByName(name);

            if (guild == null)
            {
                return null;
            }

            return new GuildDetailsViewModel
            {
                Guild = guild,
                Members = new CharacterService().GetAllCharacters()
                    .Where(c => guild.Members.Contains(c.CharacterName))
                    .ToList()
            };
        }

        public GuildFormViewModel GetGuildCreateForm()
        {
            GuildFormViewModel form = new GuildFormViewModel();
            PopulateOptions(form);
            return form;
        }

        public GuildFormViewModel? GetGuildEditForm(string name)
        {
            Guild? guild = GetGuildByName(name);

            if (guild == null)
            {
                return null;
            }

            GuildFormViewModel form = new GuildFormViewModel
            {
                OriginalGuildName = guild.GuildName,
                GuildName = guild.GuildName,
                Creator = guild.Creator,
                CreationDate = guild.CreationDate,
                BenefitsText = string.Join(", ", guild.Benefits),
                SelectedMembers = new List<string>(guild.Members)
            };

            PopulateOptions(form);
            return form;
        }

        public void AddGuild(GuildFormViewModel form)
        {
            Guild guild = new Guild
            {
                GuildName = form.GuildName.Trim(),
                Creator = form.Creator.Trim(),
                CreationDate = form.CreationDate.Trim(),
                Benefits = SplitList(form.BenefitsText),
                Members = CleanList(form.SelectedMembers)
            };

            guild.MemberCount = guild.Members.Count;

            var coll = MongoConnection.Database.GetCollection<Guild>("Guilds");
            coll.InsertOne(guild);

            SyncCharacterMemberships(guild.GuildName, guild.Members, new List<string>());
        }

        public void UpdateGuild(GuildFormViewModel form)
        {
            Guild? guild = GetGuildByName(form.OriginalGuildName);

            if (guild == null)
            {
                return;
            }

            string oldName = guild.GuildName;
            List<string> oldMembers = new List<string>(guild.Members);
            string newName = form.GuildName.Trim();

            Console.WriteLine(guild.GuildName);
            Console.WriteLine(form.GuildName);

            var coll = MongoConnection.Database.GetCollection<Guild>("Guilds");
            var filter = Builders<Guild>.Filter.Eq("GuildName", form.OriginalGuildName);
            var combinedUpdate = Builders<Guild>.Update.Combine(
                Builders<Guild>.Update.Set("GuildName", form.GuildName),
                Builders<Guild>.Update.Set("Creator", form.Creator),
                Builders<Guild>.Update.Set("MemberCount", form.SelectedMembers.Count),
                Builders<Guild>.Update.Set("Members", form.SelectedMembers),
                Builders<Guild>.Update.Set("CreationDate", form.CreationDate),
                Builders<Guild>.Update.Set("Benefits", SplitList(form.BenefitsText))
            );
            coll.UpdateOne(filter, combinedUpdate);

            SyncCharacterMemberships(newName, guild.Members, oldMembers);
        }

        public void UpdateGuild(Guild form)
        {
            Guild? guild = GetGuildByName(form.GuildName);

            if (guild == null)
            {
                return;
            }

            string oldName = guild.GuildName;
            List<string> oldMembers = new List<string>(guild.Members);
            string newName = form.GuildName.Trim();

            var coll = MongoConnection.Database.GetCollection<Guild>("Guilds");
            var filter = Builders<Guild>.Filter.Eq("GuildName", guild.GuildName);
            var combinedUpdate = Builders<Guild>.Update.Combine(
                Builders<Guild>.Update.Set("GuildName", form.GuildName),
                Builders<Guild>.Update.Set("Creator", form.Creator),
                Builders<Guild>.Update.Set("MemberCount", form.MemberCount),
                Builders<Guild>.Update.Set("Members", form.Members),
                Builders<Guild>.Update.Set("CreationDate", form.CreationDate),
                Builders<Guild>.Update.Set("Benefits", form.Benefits)
            );
            coll.UpdateOne(filter, combinedUpdate);

            SyncCharacterMemberships(newName, guild.Members, oldMembers);
        }

        public void DeleteGuild(string name)
        {
            Guild? guild = GetGuildByName(name);

            if (guild == null)
            {
                return;
            }

            {
                var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
                foreach (var character in new CharacterService().GetAllCharacters())
                {
                    character.GuildMemberships.RemoveAll(g => g == guild.GuildName);
                    var filter = Builders<PlayerCharacter>.Filter.Empty;
                    var combinedUpdate = Builders<PlayerCharacter>.Update.Combine(
                        Builders<PlayerCharacter>.Update.Set("GuildMemberships", character.GuildMemberships)
                    );
                    coll.UpdateOne(filter, combinedUpdate);
                }
            }

            {
                var coll = MongoConnection.Database.GetCollection<Guild>("Guild");
                var filter = Builders<Guild>.Filter.Eq("GuildName", guild.GuildName);
                coll.DeleteOne(filter);
            }
        }

        private void PopulateOptions(GuildFormViewModel form)
        {
            form.MemberOptions = new CharacterService().GetAllCharacters()
                .Select(c => new SelectListItem { Value = c.CharacterName, Text = c.CharacterName })
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

        private void SyncCharacterMemberships(string guildName, List<string> newMembers, List<string> oldMembers)
        {
            var characterService = new CharacterService();
            var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            foreach (var character in characterService.GetAllCharacters())
            {
                bool shouldBeMember = newMembers.Contains(character.CharacterName);
                bool wasMember = oldMembers.Contains(character.CharacterName) || character.GuildMemberships.Contains(guildName);

                if (shouldBeMember && !character.GuildMemberships.Contains(guildName))
                {
                    character.GuildMemberships.Add(guildName);
                }

                if (!shouldBeMember && wasMember)
                {
                    character.GuildMemberships.RemoveAll(g => g == guildName);
                }

                var filter = Builders<PlayerCharacter>.Filter.Empty;
                var combinedUpdate = Builders<PlayerCharacter>.Update.Combine(
                    Builders<PlayerCharacter>.Update.Set("GuildMemberships", character.GuildMemberships)
                );
                coll.UpdateOne(filter, combinedUpdate);
            }
        }
    }
}
