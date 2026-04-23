using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class GuildDetailsViewModel
    {
        public Guild Guild { get; set; } = new Guild();
        public List<PlayerCharacter> Members { get; set; } = new List<PlayerCharacter>();
    }
}