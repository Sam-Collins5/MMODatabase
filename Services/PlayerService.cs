using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Services
{
    public class PlayerService : IPlayerService
    {
        public List<Player> GetAllPlayers()
        {
            return FakeGameData.Players;
        }

        public Player? GetPlayerByUserName(string username)
        {
            return FakeGameData.Players
                .FirstOrDefault(p => p.UserName == username);
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
                CharacterDetails = FakeGameData.Characters
                    .Where(c => c.PlayerName == player.PlayerName)
                    .ToList(),
                Transactions = FakeGameData.Transactions
                    .Where(t => t.PlayerName == player.PlayerName)
                    .ToList()
            };
        }

        public void AddPlayer(Player player)
        {
            player.Characters ??= new List<string>();
            FakeGameData.Players.Add(player);
        }

        public void UpdatePlayer(Player player)
        {
            Player? existingPlayer = GetPlayerByUserName(player.UserName);

            if (existingPlayer == null)
            {
                return;
            }

            string oldPlayerName = existingPlayer.PlayerName;

            existingPlayer.PlayerName = player.PlayerName;
            existingPlayer.Age = player.Age;
            existingPlayer.PasswordHash = player.PasswordHash;
            existingPlayer.MemberSince = player.MemberSince;
            existingPlayer.Characters = player.Characters ?? new List<string>();

            if (!string.Equals(oldPlayerName, player.PlayerName, StringComparison.Ordinal))
            {
                foreach (var character in FakeGameData.Characters.Where(c => c.PlayerName == oldPlayerName))
                {
                    character.PlayerName = player.PlayerName;
                }

                foreach (var transaction in FakeGameData.Transactions.Where(t => t.PlayerName == oldPlayerName))
                {
                    transaction.PlayerName = player.PlayerName;
                }
            }
        }

        public void DeletePlayer(string username)
        {
            Player? player = GetPlayerByUserName(username);

            if (player == null)
            {
                return;
            }

            FakeGameData.Characters.RemoveAll(c => c.PlayerName == player.PlayerName);
            FakeGameData.Transactions.RemoveAll(t => t.PlayerName == player.PlayerName);
            FakeGameData.Players.Remove(player);
        }
    }
}
