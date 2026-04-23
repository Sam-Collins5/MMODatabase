using MMOngo.Models;

namespace MMOngo.Services.Interfaces
{
    public interface INpcService
    {
        List<Npc> GetAllNpcs();
        Npc? GetNpcByName(string name);
    }
}