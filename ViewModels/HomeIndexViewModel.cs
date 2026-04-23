using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
        public List<Mission> Missions { get; set; } = new List<Mission>();
    }
}