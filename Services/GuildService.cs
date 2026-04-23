using MMOngo.Models;
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
            return FakeGameData.Guilds
                .FirstOrDefault(g => g.GuildName == name);
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
    }
}