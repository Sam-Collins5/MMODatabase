using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface IMissionService
    {
        List<Mission> GetAllMissions();
        Mission? GetMissionByName(string name);

        MissionFormViewModel GetMissionCreateForm();
        MissionFormViewModel? GetMissionEditForm(string name);

        void AddMission(MissionFormViewModel form);
        void UpdateMission(MissionFormViewModel form);
        void DeleteMission(string name);
    }
}