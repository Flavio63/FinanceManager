using FinanceManager.Models;
namespace FinanceManager.Services
{
    public interface IRegistryServices
    {
        #region Owner
        RegistryOwnersList GetRegistryOwners();
        RegistryOwner GetOwnerByName(string name);
        RegistryOwner GetOwnerById(int id);
        void UpdateOwner(RegistryOwner owner);
        void AddOwner(string name);
        void DeleteOwner(int id);
        #endregion

        #region ShareType
        RegistryShareTypeList GetRegistryShareTypeList();
        RegistryShareType GetRegistryShareTypeByName(string name);
        RegistryShareType GetRegistryShareTypeById(int id);
        void UpdateShareType(RegistryShareType registryShareType);
        void AddShareType(string description);
        void DeleteShareType(int id);
        #endregion

        #region Currency
        RegistryShareTypeList GetRegistryCurrencyList();
        RegistryShareType GetRegistryCurrencyByName(string name);
        RegistryShareType GetRegistryCurrencyById(int id);
        void UpdateCurrency(RegistryShareType registryShareType);
        void AddCurrency(string description);
        void DeleteCurrency(int id);
        #endregion
    }
}
