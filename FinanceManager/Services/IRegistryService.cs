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

        #region Location
        RegistryLocationList GetRegistryLocationList();
        RegistryLocation GetRegistryLocationByName(string name);
        RegistryLocation GetRegistryLocationById(int id);
        void UpdateLocation(RegistryLocation registryLocation);
        void AddLocation(string description);
        void DeleteLocation(int id);
        #endregion

        #region Firm
        RegistryFirmList GetRegistryFirmList();
        RegistryFirm GetRegistryFirmByName(string name);
        RegistryFirm GetRegistryFirmById(int id);
        void UpdateFirm(RegistryFirm registryFirm);
        void AddFirm(string description);
        void DeleteFirm(int id);
        #endregion

        #region Market
        RegistryMarketList GetRegistryMarketList();
        RegistryMarket GetRegistryMarketByName(string name);
        RegistryMarket GetRegistryMarketById(int id);
        void UpdateMarket(RegistryMarket registryMarket);
        void AddMarket(string description);
        void DeleteMarket(int id);
        #endregion

        #region Currency
        RegistryCurrencyList GetRegistryCurrencyList();
        RegistryCurrency GetRegistryCurrencyByName(string name);
        RegistryCurrency GetRegistryCurrencyById(int id);
        void UpdateCurrency(RegistryCurrency registryCurrency);
        void AddCurrency(RegistryCurrency registryCurrency);
        void DeleteCurrency(int id);
        #endregion

        #region Share
        RegistryShareList GetRegistryShareList();
        RegistryShareList GetRegistryByShareType(int shareTypeID);
        RegistryShareList GetRegistryByCurrency(int currencyID);
        RegistryShareList GetRegistryByFirm(int firmID);
        RegistryShare GetRegistryShareByName(string name);
        RegistryShare GetRegistryShareById(int id);
        RegistryShare GetRegistryShareByIsin(string isin);
        void UpdateShare(RegistryShare registryShare);
        void AddShare(RegistryShare registryShare);
        void DeleteShare(int id);
        #endregion
    }
}
