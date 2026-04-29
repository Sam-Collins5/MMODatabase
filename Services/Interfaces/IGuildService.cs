using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface IGuildService
    {
        List<Guild> GetAllGuilds();
        Guild? GetGuildByName(string name);
        GuildDetailsViewModel? GetGuildDetails(string name);

        GuildFormViewModel GetGuildCreateForm();
        GuildFormViewModel? GetGuildEditForm(string name);

        void AddGuild(GuildFormViewModel form);
        void UpdateGuild(GuildFormViewModel form);
        void DeleteGuild(string name);
    }
}