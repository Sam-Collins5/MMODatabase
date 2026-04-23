using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class NpcService : INpcService
    {
        public List<Npc> GetAllNpcs()
        {
            var npcColl = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var npcFilter = Builders<Npc>.Filter.Empty;

            return npcColl.Find(npcFilter).ToList();
        }

        public Npc? GetNpcByName(string name)
        {
            return FakeGameData.Npcs
                .FirstOrDefault(n => n.NpcName == name);
        }
    }
}