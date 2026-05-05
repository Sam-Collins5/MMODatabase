using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;


namespace MMOngo.Services.Testing
{
    public class FakePlayerService : IPlayerService
    {
        private static readonly List<Player> Players = new List<Player>
        {
            new Player
            {
                UserName = "testuser",
                PlayerName = "Test Player",
                Age = 20,
                PasswordHash = "fake-password-hash",
                MemberSince = DateTime.Now.ToString(),
                Characters = new List<string> { "Warrior", "Mage" }
            },
            new Player
            {
                UserName = "admin",
                PlayerName = "Admin Player",
                Age = 22,
                PasswordHash = "fake-password-hash",
                MemberSince = DateTime.Now.ToString(),
                Characters = new List<string> { "Knight" }
            }
        };

        private static readonly List<PlayerCharacter> PlayerCharacters = new List<PlayerCharacter>();

        private static readonly List<Transaction> Transactions = new List<Transaction>();

        public List<Player> GetAllPlayers()
        {
            return Players;
        }

        public Player? GetPlayerByUserName(string username)
        {
            return Players.FirstOrDefault(player =>
                player.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public PlayerDetailsViewModel? GetPlayerDetails(string username)
        {
            Player? player = GetPlayerByUserName(username);

            if (player == null)
            {
                return null;
            }

            return new PlayerDetailsViewModel
            {
                Player = player,
                CharacterDetails = PlayerCharacters
                    .Where(character => character.UserName.Equals(player.UserName, StringComparison.OrdinalIgnoreCase))
                    .ToList(),
                Transactions = Transactions
                    .Where(transaction => transaction.UserName.Equals(player.UserName, StringComparison.OrdinalIgnoreCase))
                    .ToList()
            };
        }

        public void AddPlayer(Player player)
        {
            Player? existingPlayer = GetPlayerByUserName(player.UserName);

            if (existingPlayer != null)
            {
                return;
            }

            player.Characters ??= new List<string>();

            Players.Add(player);
        }

        public void UpdatePlayer(Player player)
        {
            Player? existingPlayer = GetPlayerByUserName(player.UserName);

            if (existingPlayer == null)
            {
                return;
            }

            existingPlayer.UserName = player.UserName;
            existingPlayer.PlayerName = player.PlayerName;
            existingPlayer.Age = player.Age;
            existingPlayer.PasswordHash = player.PasswordHash;
            existingPlayer.MemberSince = player.MemberSince;
            existingPlayer.Characters = player.Characters ?? new List<string>();
        }

        public void DeletePlayer(string username)
        {
            Player? player = GetPlayerByUserName(username);

            if (player == null)
            {
                return;
            }

            Players.Remove(player);

            PlayerCharacters.RemoveAll(character =>
                character.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

            Transactions.RemoveAll(transaction =>
                transaction.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}