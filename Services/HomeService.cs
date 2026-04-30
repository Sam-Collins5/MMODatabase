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
                Guilds = new GuildService().GetAllGuilds(),
                Missions = new MissionService().GetAllMissions()
            };
        }
    }
}