using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;
using MongoDB.Driver;

namespace MMOngo.Services
{
    public class ShopService : IShopService
    {
        public ShopViewModel GetShopData()
        {
            var weaponColl = MongoConnection.Database.GetCollection<Weapon>("Weapons");
            var weaponFilter = Builders<Weapon>.Filter.Empty;

            var armorColl = MongoConnection.Database.GetCollection<Armor>("Armors");
            var armorFilter = Builders<Armor>.Filter.Empty;

            var toolColl = MongoConnection.Database.GetCollection<ToolItem>("Tools");
            var toolFilter = Builders<ToolItem>.Filter.Empty;

            var spellColl = MongoConnection.Database.GetCollection<Spell>("Spells");
            var spellFilter = Builders<Spell>.Filter.Empty;

            return new ShopViewModel
            {
                Weapons = weaponColl.Find(weaponFilter).ToList(),
                Armors = armorColl.Find(armorFilter).ToList(),
                Tools = toolColl.Find(toolFilter).ToList(),
                Spells = spellColl.Find(spellFilter).ToList(),

            };
        }
    }
}