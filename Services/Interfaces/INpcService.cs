using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface INpcService
    {
        List<Npc> GetAllNpcs();
        Npc? GetNpcByName(string name);

        NpcFormViewModel GetNpcCreateForm();
        NpcFormViewModel? GetNpcEditForm(string name);

        void AddNpc(NpcFormViewModel form);
        void UpdateNpc(NpcFormViewModel form);
        void DeleteNpc(string name);
    }
}