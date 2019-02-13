using FinanceManager.Models;
namespace FinanceManager.Services
{
    public interface IRegistryServices
    {
        #region Owner
        RegistryOwnersList GetGestioneList();
        void UpdateGestioneName(RegistryOwner owner);
        void AddGestione(string name);
        void DeleteGestione(int id);
        RegistryOwner GetGestione(int id);
        #endregion

        #region ShareType
        RegistryShareTypeList GetRegistryShareTypeList();
        void UpdateShareType(RegistryShareType registryShareType);
        void AddShareType(string description);
        void DeleteShareType(uint id);
        #endregion

        #region Location
        RegistryLocationList GetRegistryLocationList();
        void UpdateLocation(RegistryLocation registryLocation);
        void AddLocation(string description);
        void DeleteLocation(int id);
        RegistryLocation GetLocation(int id);
        #endregion

        #region Firm
        RegistryFirmList GetRegistryFirmList();
        void UpdateFirm(RegistryFirm registryFirm);
        void AddFirm(string description);
        void DeleteFirm(uint id);
        #endregion

        #region Currency
        RegistryCurrencyList GetRegistryCurrencyList();
        void UpdateCurrency(RegistryCurrency registryCurrency);
        void AddCurrency(RegistryCurrency registryCurrency);
        void DeleteCurrency(int id);
        #endregion

        #region Share
        RegistryShareList GetRegistryShareList();
        RegistryShareList GetSharesByType(uint idShareType);
        void UpdateShare(RegistryShare registryShare);
        void AddShare(RegistryShare registryShare);
        void DeleteShare(uint id);
        #endregion

        #region MovementType
        RegistryMovementTypeList GetRegistryMovementTypesList();
        void UpdateMovementType(RegistryMovementType registryMovementType);
        void AddMovementType(string name);
        void DeleteMovementType(int id);
        RegistryMovementType GetMovementType(int id);
        #endregion

        TipoSoldiList GetTipoSoldiList();
        TipoSoldi GetTipoSoldiById(int id);
    }
}
