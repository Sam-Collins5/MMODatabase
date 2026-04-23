using MMOngo.Models;
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
                Players = FakeGameData.Players,
                Characters = FakeGameData.Characters,
                Guilds = FakeGameData.Guilds,
                Missions = FakeGameData.Missions
            };
        }
    }
}