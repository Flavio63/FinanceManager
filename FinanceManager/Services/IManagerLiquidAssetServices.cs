using FinanceManager.Models;
using System.Data;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerAndLocation(int IdOwner, int idLocation);
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdMovement);
        ManagerLiquidAssetList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        ManagerLiquidAsset GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        double GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency);
        ManagerLiquidAssetList GetShareMovements(int IdOwner, int IdLocation, uint IdShare);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare);
        double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);
        void InsertAccountMovement(ContoCorrente contoCorrente);
        QuoteList GetQuote();   // Calcolo le quote per investitore
        InvestitoreList GetInvestitori();
        QuoteTabList GetQuoteTab();     //Prendo tutte i record della tabella Quote
        void AddGiroconto(); // coinvolge anche InsertAccountMovement questo è il movimento 12
        void UpdateQuoteTab(int idQuote);   // aggiorna solo i movimenti 1 e 2
        void UpdateGiroconto(int idQuote);     // coinvolge anche il conco corrente da fare
    }
}
