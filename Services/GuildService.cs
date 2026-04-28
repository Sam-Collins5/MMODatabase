using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Services
{
    public class GuildService : IGuildService
    {
        public List<Guild> GetAllGuilds()
        {
            return FakeGameData.Guilds;
        }

        public Guild? GetGuildByName(string name)
        {
            return FakeGameData.Guilds.FirstOrDefault(g => g.GuildName == name);
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
                Members = FakeGameData.Characters
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
            FakeGameData.Guilds.Add(guild);
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

            guild.GuildName = newName;
            guild.Creator = form.Creator.Trim();
            guild.CreationDate = form.CreationDate.Trim();
            guild.Benefits = SplitList(form.BenefitsText);
            guild.Members = CleanList(form.SelectedMembers);
            guild.MemberCount = guild.Members.Count;

            if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                foreach (var character in FakeGameData.Characters)
                {
                    for (int i = 0; i < character.GuildMemberships.Count; i++)
                    {
                        if (character.GuildMemberships[i] == oldName)
                        {
                            character.GuildMemberships[i] = newName;
                        }
                    }
                }
            }

            SyncCharacterMemberships(newName, guild.Members, oldMembers);
        }

        public void DeleteGuild(string name)
        {
            Guild? guild = GetGuildByName(name);

            if (guild == null)
            {
                return;
            }

            foreach (var character in FakeGameData.Characters)
            {
                character.GuildMemberships.RemoveAll(g => g == guild.GuildName);
            }

            FakeGameData.Guilds.Remove(guild);
        }

        private void PopulateOptions(GuildFormViewModel form)
        {
            form.MemberOptions = FakeGameData.Characters
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
            foreach (var character in FakeGameData.Characters)
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
            }
        }
    }
}
