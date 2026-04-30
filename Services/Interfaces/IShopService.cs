using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface IShopService
    {
        ShopViewModel GetShopData();

        ShopItemFormViewModel GetItemCreateForm(string category);
        ShopItemFormViewModel? GetItemEditForm(string category, int id);
        object? GetItemForDelete(string category, int id);

        void AddItem(ShopItemFormViewModel form);
        void UpdateItem(ShopItemFormViewModel form);
        void DeleteItem(string category, int id);
    }
}