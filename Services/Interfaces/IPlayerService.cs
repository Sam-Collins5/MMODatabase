using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface IPlayerService
    {
        List<Player> GetAllPlayers();
        Player? GetPlayerByUserName(string username);
        PlayerDetailsViewModel? GetPlayerDetails(string username);
        void AddPlayer(Player player);
        void UpdatePlayer(Player player);
        void DeletePlayer(string username);
    }
}
