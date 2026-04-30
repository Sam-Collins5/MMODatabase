using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

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
            return FakeGameData.Missions.FirstOrDefault(m => m.MissionName == name);
        }

        public MissionFormViewModel GetMissionCreateForm()
        {
            MissionFormViewModel form = new MissionFormViewModel();
            PopulateOptions(form);
            return form;
        }

        public MissionFormViewModel? GetMissionEditForm(string name)
        {
            Mission? mission = GetMissionByName(name);

            if (mission == null)
            {
                return null;
            }

            MissionFormViewModel form = new MissionFormViewModel
            {
                OriginalMissionName = mission.MissionName,
                MissionName = mission.MissionName,
                MissionType = mission.MissionType,
                QuestGiver = mission.QuestGiver,
                Reward = mission.Reward
            };

            PopulateOptions(form);
            return form;
        }

        public void AddMission(MissionFormViewModel form)
        {
            FakeGameData.Missions.Add(new Mission
            {
                MissionName = form.MissionName.Trim(),
                MissionType = form.MissionType.Trim(),
                QuestGiver = form.QuestGiver.Trim(),
                Reward = form.Reward
            });
        }

        public void UpdateMission(MissionFormViewModel form)
        {
            Mission? mission = GetMissionByName(form.OriginalMissionName);

            if (mission == null)
            {
                return;
            }

            string oldName = mission.MissionName;
            string newName = form.MissionName.Trim();

            mission.MissionName = newName;
            mission.MissionType = form.MissionType.Trim();
            mission.QuestGiver = form.QuestGiver.Trim();
            mission.Reward = form.Reward;

            if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                foreach (var character in FakeGameData.Characters)
                {
                    ReplaceName(character.CurrentMissions, oldName, newName);
                    ReplaceName(character.CompletedMissions, oldName, newName);
                }

                foreach (var npc in FakeGameData.Npcs)
                {
                    ReplaceName(npc.Quests, oldName, newName);
                }
            }
        }

        public void DeleteMission(string name)
        {
            Mission? mission = GetMissionByName(name);

            if (mission == null)
            {
                return;
            }

            foreach (var character in FakeGameData.Characters)
            {
                character.CurrentMissions.RemoveAll(m => m == mission.MissionName);
                character.CompletedMissions.RemoveAll(m => m == mission.MissionName);
            }

            foreach (var npc in FakeGameData.Npcs)
            {
                npc.Quests.RemoveAll(q => q == mission.MissionName);
            }

            FakeGameData.Missions.Remove(mission);
        }

        private void PopulateOptions(MissionFormViewModel form)
        {
            form.QuestGiverOptions = FakeGameData.Npcs
                .Select(n => new SelectListItem { Value = n.NpcName, Text = n.NpcName })
                .ToList();
        }

        private void ReplaceName(List<string> values, string oldName, string newName)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == oldName)
                {
                    values[i] = newName;
                }
            }
        }
    }
}
