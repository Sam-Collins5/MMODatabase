using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class PlayerService : IPlayerService
    {
        public List<Player> GetAllPlayers()
        {
            var coll = MongoConnection.Database.GetCollection<Player>("Players");
            
            var filter = Builders<Player>.Filter.Empty;
            return coll.Find(filter).ToList();
        }

        public Player? GetPlayerByUserName(string username)
        {
            var coll = MongoConnection.Database.GetCollection<Player>("Players");

            var filter = Builders<Player>.Filter.Empty;
            return coll.Find(filter).ToList().FirstOrDefault(p => p.UserName == username);
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
            var coll = MongoConnection.Database.GetCollection<Player>("Players");
            player.Characters ??= new List<string>();
            coll.InsertOne(player);
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

            var coll = MongoConnection.Database.GetCollection<Player>("Players");
            var filter = Builders<Player>.Filter.Eq("PlayerName", oldPlayerName);
            var combinedUpdate = Builders<Player>.Update.Combine(
                Builders<Player>.Update.Set("Age", player.Age),
                Builders<Player>.Update.Set("PasswordHash", player.PasswordHash),
                Builders<Player>.Update.Set("MemberSince", player.MemberSince),
                Builders<Player>.Update.Set("Characters", player.Characters)
            );



            //if (!string.Equals(oldPlayerName, player.PlayerName, StringComparison.Ordinal))
            //{
            //    foreach (var character in FakeGameData.Characters.Where(c => c.PlayerName == oldPlayerName))
            //    {
            //        character.PlayerName = player.PlayerName;
            //    }

            //    foreach (var transaction in FakeGameData.Transactions.Where(t => t.PlayerName == oldPlayerName))
            //    {
            //        transaction.PlayerName = player.PlayerName;
            //    }
            //}
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
