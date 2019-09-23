using FinanceManager.Models;
namespace FinanceManager.Services
{
    public interface IRegistryServices
    {
        #region Owner
        /// <summary>
        /// Ritorna tutti i nominativi legati alla
        /// gestione dei conti
        /// </summary>
        /// <returns>Observable Collection</returns>
        RegistryOwnersList GetGestioneList();
        /// <summary>
        /// Aggiorna i dati di un gestore
        /// </summary>
        /// <param name="owner">Il record da aggiornare</param>
        void UpdateGestioneName(RegistryOwner owner);
        /// <summary>
        /// Aggiunge una voce alla tabella gestioni
        /// </summary>
        /// <param name="name">Il nome della persona</param>
        /// <param name="tipologia">La tipologia</param>
        void AddGestione(string name, string tipologia);
        void DeleteGestione(int id);
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
        void AddLocation(string description, string note);
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
        RegistryShareList GetSharesByType(int[] id_tipo_titolo);
        void UpdateShare(RegistryShare registryShare);
        void AddShare(RegistryShare registryShare);
        void DeleteShare(uint id);
        RegistryShare GetShareById(uint id);
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
