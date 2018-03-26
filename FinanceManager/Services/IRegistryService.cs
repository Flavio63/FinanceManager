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
        RegistryCurrencyList GetRegistryCurrencyList();
        RegistryCurrency GetRegistryCurrencyByName(string name);
        RegistryCurrency GetRegistryCurrencyById(int id);
        void UpdateCurrency(RegistryCurrency registryCurrency);
        void AddCurrency(RegistryCurrency registryCurrency);
        void DeleteCurrency(int id);
        #endregion
    }
}
