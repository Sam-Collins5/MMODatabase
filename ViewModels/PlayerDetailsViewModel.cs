using MMOngo.Models;

namespace MMOngo.ViewModels
{
    public class PlayerDetailsViewModel
    {
        public Player Player { get; set; } = new Player();
        public List<PlayerCharacter> CharacterDetails { get; set; } = new List<PlayerCharacter>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}