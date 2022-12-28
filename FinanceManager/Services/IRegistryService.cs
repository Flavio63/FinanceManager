using FinanceManager.Models;
namespace FinanceManager.Services
{
    public interface IRegistryServices
    {
        #region Soci
        /// <summary>
        /// Ritorna tutti i nominativi legati alla
        /// gestione dei socii
        /// </summary>
        /// <returns>Observable Collection</returns>
        RegistrySociList GetSociList();
        /// <summary>
        /// Aggiorna il nome di un socio
        /// </summary>
        /// <param name="socioName">Il nuovo nome</param>
        void UpdateSocioName(RegistrySoci socio);
        /// <summary>
        /// Aggiunge una voce alla tabella soci
        /// </summary>
        /// <param name="socio">Il record da aggiungere</param>
        void AddSocio(RegistrySoci socio);
        void DeleteSocio(int id_socio);
        #endregion

        #region Gestioni
        /// <summary>
        /// Ritorna tutti i nominativi legati alle
        /// gestioni dei conti
        /// </summary>
        /// <returns>Observable Collection</returns>
        RegistryGestioniList GetGestioneList();
        /// <summary>
        /// Aggiorna i dati di una gestione
        /// </summary>
        /// <param name="gestione">Il record da aggiornare</param>
        void UpdateGestioneName(RegistryGestioni gestione);
        /// <summary>
        /// Aggiunge una voce alla tabella gestioni
        /// </summary>
        /// <param name="gestione">Il record da aggiungere</param>
        void AddGestione(RegistryGestioni gestione);
        /// <summary>
        /// Elimina una voce dalla tabella gestioni
        /// </summary>
        /// <param name="id_gestione">Il record da aggiungere</param>
        void DeleteGestione(int id_gestione);
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
        RegistryShareList GetSharesByFirms(int[] id_aziende);
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
        #endregion

        #region TIPO GESTIONE UTILI
        RegistryTipoGestioniUtiliList GetTipoGestioniUtiliList();
        void UpdateTipoGestioniUtili(RegistryTipoGestioniUtili tipoGestioniUtili);
        void InsertTipoGestioniUtili(RegistryTipoGestioniUtili tipoGestioniUtili);
        void DeleteTipoGestioniUtili(int id_tipoGestioniUtili);

        #endregion

        TipoSoldiList GetTipoSoldiList();
    }
}
