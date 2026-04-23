using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Services
{
    public class ShopService : IShopService
    {
        public ShopViewModel GetShopData()
        {
            return new ShopViewModel
            {
                Weapons = FakeGameData.Weapons,
                Armors = FakeGameData.Armors,
                Tools = FakeGameData.Tools,
                Spells = FakeGameData.Spells
            };
        }
    }
}