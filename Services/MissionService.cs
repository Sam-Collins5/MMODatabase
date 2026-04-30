using Microsoft.AspNetCore.Mvc.Rendering;
using MMOngo.Models;
using MMOngo.Models.Test;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Numerics;

namespace MMOngo.Services
{
    public class MissionService : IMissionService
    {
        public List<Mission> GetAllMissions()
        {
            var coll = MongoConnection.Database.GetCollection<Mission>("Missions");

            var filter = Builders<Mission>.Filter.Empty;
            return coll.Find(filter).ToList();
        }

        public Mission? GetMissionByName(string name)
        {
            var coll = MongoConnection.Database.GetCollection<Mission>("Missions");

            var filter = Builders<Mission>.Filter.Empty;
            return coll.Find(filter).ToList().FirstOrDefault(m => m.MissionName == name);
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
            Mission mission = new Mission();
            mission.MissionName = form.MissionName.Trim();
            mission.MissionType = form.MissionType;
            mission.QuestGiver = form.QuestGiver.Trim();
            mission.Reward = form.Reward;
            var coll = MongoConnection.Database.GetCollection<Mission>("Missions");
            coll.InsertOne(mission);
        }

        public void UpdateMission(MissionFormViewModel form)
        {
            Mission? mission = GetMissionByName(form.OriginalMissionName);

            if (mission == null)
            {
                return;
            }

            var coll = MongoConnection.Database.GetCollection<Mission>("Missions");
            var filter = Builders<Mission>.Filter.Eq("MissionName", mission.MissionName);
            var combinedUpdate = Builders<Mission>.Update.Combine(
                Builders<Mission>.Update.Set("MissionName", form.MissionName),
                Builders<Mission>.Update.Set("MissionType", form.MissionType),
                Builders<Mission>.Update.Set("QuestGiver", form.QuestGiver),
                Builders<Mission>.Update.Set("Reward", form.Reward)
            );
            coll.UpdateOne(filter, combinedUpdate);
        }

        public void DeleteMission(string name)
        {
            Mission? mission = GetMissionByName(name);

            if (mission == null)
            {
                return;
            }

            {
                var characterService = new CharacterService();
                var characters = characterService.GetAllCharacters();
                foreach (var character in characters)
                {
                    character.CurrentMissions.RemoveAll(m => m == mission.MissionName);
                    character.CompletedMissions.RemoveAll(m => m == mission.MissionName);
                    characterService.UpdateCharacter(character);
                }
            }

            {
                var coll = MongoConnection.Database.GetCollection<Mission>("Missions");
                var filter = Builders<Mission>.Filter.Eq("MissionName", mission.MissionName);
                coll.DeleteOne(filter);
            }
        }

        private void PopulateOptions(MissionFormViewModel form)
        {
            var coll = MongoConnection.Database.GetCollection<Npc>("NPCs");
            var filter = Builders<Npc>.Filter.Empty;
            form.QuestGiverOptions = coll.Find(filter).ToList()
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
