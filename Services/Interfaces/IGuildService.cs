using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface IGuildService
    {
        List<Guild> GetAllGuilds();
        Guild? GetGuildByName(string name);
        GuildDetailsViewModel? GetGuildDetails(string name);
    }
}