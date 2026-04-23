using MMOngo.Models;

namespace MMOngo.Services.Interfaces
{
    public interface IMissionService
    {
        List<Mission> GetAllMissions();
        Mission? GetMissionByName(string name);
    }
}