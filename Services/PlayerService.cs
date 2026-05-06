using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class PlayerService : IPlayerService
    {
        public List<Player> GetAllPlayers()
        {
            if (MongoConnection.Database == null)
            {
                return new List<Player>();
            }

            var coll = MongoConnection.Database.GetCollection<Player>("Players");

            var filter = Builders<Player>.Filter.Empty;
            return coll.Find(filter).ToList();
        }

        public Player? GetPlayerByUserName(string username)
        {
            if (MongoConnection.Database == null)
            {
                return null;
            }

            var coll = MongoConnection.Database.GetCollection<Player>("Players");

            var filter = Builders<Player>.Filter.Eq(player => player.UserName, username);
            return coll.Find(filter).FirstOrDefault();
        }

        public PlayerDetailsViewModel? GetPlayerDetails(string username)
        {
            if (MongoConnection.Database == null)
            {
                return null;
            }

            Player? player = GetPlayerByUserName(username);

            if (player == null)
            {
                return null;
            }

            var characterColl = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
            var transactionColl = MongoConnection.Database.GetCollection<Transaction>("Transactions");

            var characterFilter = Builders<PlayerCharacter>.Filter.Eq(character => character.UserName, player.UserName);
            var transactionFilter = Builders<Transaction>.Filter.Eq(transaction => transaction.UserName, player.UserName);

            return new PlayerDetailsViewModel
            {
                Player = player,
                CharacterDetails = characterColl.Find(characterFilter).ToList(),
                Transactions = transactionColl.Find(transactionFilter).ToList()
            };
        }

        public void AddPlayer(Player player)
        {
            if (MongoConnection.Database == null)
            {
                return;
            }

            var coll = MongoConnection.Database.GetCollection<Player>("Players");

            player.Characters ??= new List<string>();

            coll.InsertOne(player);
        }

        public void UpdatePlayer(Player player)
        {
            if (MongoConnection.Database == null)
            {
                return;
            }

            Player? existingPlayer = GetPlayerByUserName(player.UserName);

            if (existingPlayer == null)
            {
                return;
            }

            string oldUserName = existingPlayer.UserName;

            var coll = MongoConnection.Database.GetCollection<Player>("Players");

            var filter = Builders<Player>.Filter.Eq(currentPlayer => currentPlayer.UserName, oldUserName);

            var combinedUpdate = Builders<Player>.Update.Combine(
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.UserName, player.UserName),
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.PlayerName, player.PlayerName),
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.Age, player.Age),
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.PasswordHash, player.PasswordHash),
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.MemberSince, player.MemberSince),
                Builders<Player>.Update.Set(currentPlayer => currentPlayer.Characters, player.Characters)
            );

            coll.UpdateOne(filter, combinedUpdate);

            var characterColl = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");

            var characterFilter = Builders<PlayerCharacter>.Filter.Eq(character => character.UserName, oldUserName);

            var characterUpdate = Builders<PlayerCharacter>.Update.Set(character => character.UserName, player.UserName);

            characterColl.UpdateMany(characterFilter, characterUpdate);

            var transactionColl = MongoConnection.Database.GetCollection<Transaction>("Transactions");

            var transactionFilter = Builders<Transaction>.Filter.Eq(transaction => transaction.UserName, oldUserName);

            var transactionUpdate = Builders<Transaction>.Update.Set(transaction => transaction.UserName, player.UserName);

            transactionColl.UpdateMany(transactionFilter, transactionUpdate);
        }

        public void DeletePlayer(string username)
        {
            if (MongoConnection.Database == null)
            {
                return;
            }

            Player? player = GetPlayerByUserName(username);

            if (player == null)
            {
                return;
            }

            {
                var coll = MongoConnection.Database.GetCollection<PlayerCharacter>("PlayerCharacters");
                var filter = Builders<PlayerCharacter>.Filter.Eq(character => character.UserName, player.UserName);
                coll.DeleteMany(filter);
            }

            {
                var coll = MongoConnection.Database.GetCollection<Transaction>("Transactions");
                var filter = Builders<Transaction>.Filter.Eq(transaction => transaction.UserName, player.UserName);
                coll.DeleteMany(filter);
            }

            {
                var coll = MongoConnection.Database.GetCollection<Player>("Players");
                var filter = Builders<Player>.Filter.Eq(currentPlayer => currentPlayer.UserName, player.UserName);
                coll.DeleteOne(filter);
            }
        }
    }
}