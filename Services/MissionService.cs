using MMOngo.Models;
using MMOngo.Services.Interfaces;

namespace MMOngo.Services
{
    public class MissionService : IMissionService
    {
        public List<Mission> GetAllMissions()
        {
            return FakeGameData.Missions;
        }

        public Mission? GetMissionByName(string name)
        {
            return FakeGameData.Missions
                .FirstOrDefault(m => m.MissionName == name);
        }
    }
}