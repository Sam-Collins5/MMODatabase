using MMOngo.Models;
using MMOngo.Services.Interfaces;

namespace MMOngo.Services
{
    public class NpcService : INpcService
    {
        public List<Npc> GetAllNpcs()
        {
            return FakeGameData.Npcs;
        }

        public Npc? GetNpcByName(string name)
        {
            return FakeGameData.Npcs
                .FirstOrDefault(n => n.NpcName == name);
        }
    }
}