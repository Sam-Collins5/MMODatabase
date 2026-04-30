using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Services
{
    public class HomeService : IHomeService
    {
        public HomeIndexViewModel GetHomeData()
        {
            return new HomeIndexViewModel
            {
                Players = new PlayerService().GetAllPlayers(),
                Characters = new CharacterService().GetAllCharacters(),
                Guilds = FakeGameData.Guilds,
                Missions = FakeGameData.Missions
            };
        }
    }
}