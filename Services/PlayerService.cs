using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

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

            var characterColl = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var characterfilter = Builders<PlayerCharacter>.Filter.Empty;

            var transactionColl = MongoConnection.Database.GetCollection<Transaction>("Transactions");
            var transactionfilter = Builders<Transaction>.Filter.Empty;

            return new PlayerDetailsViewModel
            {
                Player = player,
                CharacterDetails = characterColl.Find(characterfilter).ToList()
                    .Where(c => c.UserName == player.PlayerName)
                    .ToList(),
                Transactions = transactionColl.Find(transactionfilter).ToList()
                    .Where(t => t.UserName == player.PlayerName)
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

            string oldUserName = existingPlayer.UserName;

            var coll = MongoConnection.Database.GetCollection<Player>("Players");
            var filter = Builders<Player>.Filter.Eq("UserName", oldUserName);
            var combinedUpdate = Builders<Player>.Update.Combine(
                Builders<Player>.Update.Set("UserName", player.UserName),
                Builders<Player>.Update.Set("PlayerName", player.PlayerName),
                Builders<Player>.Update.Set("Age", player.Age),
                Builders<Player>.Update.Set("PasswordHash", player.PasswordHash),
                Builders<Player>.Update.Set("MemberSince", player.MemberSince),
                Builders<Player>.Update.Set("Characters", player.Characters)
            );
            coll.UpdateOne(filter, combinedUpdate);

            var characterColl = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacter");
            var characterFilter = Builders<PlayerCharacter>.Filter.Eq("UserName", oldUserName);
            var characters = characterColl.Find(characterFilter).ToList();
            if (characters.Count() > 0)
            {
                characters.First().UserName = player.UserName;
            }

            var transactionColl = MongoConnection.Database.GetCollection<Transaction>("Transactions");
            var transactionFilter = Builders<Transaction>.Filter.Empty;

            if (!string.Equals(oldUserName, player.PlayerName, StringComparison.Ordinal))
            {
                foreach (var transaction in transactionColl.Find(transactionFilter).ToList().Where(t => t.UserName == oldUserName))
                {
                    transaction.UserName = player.UserName;
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

            {
                var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
                var filter = Builders<PlayerCharacter>.Filter.Eq("UserName", player.UserName);
                coll.DeleteOne(filter);
            }

            {
                var coll = MongoConnection.Database.GetCollection<Transaction>("Transactions");
                var filter = Builders<Transaction>.Filter.Eq("UserName", player.UserName);
                coll.DeleteOne(filter);
            }

            {
                var coll = MongoConnection.Database.GetCollection<Player>("Players");
                var filter = Builders<Player>.Filter.Eq("UserName", player.UserName);
                coll.DeleteOne(filter);
            }
        }
    }
}
