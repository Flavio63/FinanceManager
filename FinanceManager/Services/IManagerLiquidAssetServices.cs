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
        double GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency, int[] IdMovementType);
        ManagerLiquidAssetList GetShareMovements(int IdOwner, int IdLocation, uint IdShare);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare);
        double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);

        QuoteList GetQuote();   // Calcolo le quote per investitore
        InvestitoreList GetInvestitori();

        QuoteTabList GetQuoteTab();                                     //Prendo tutti i record della tabella Quote
        void InsertInvestment(QuoteTab ActualQuote);                    // Inserisce nuovo movimento nella tabella anche il movimento 12
        void UpdateQuoteTab(QuoteTab ActualQuote);                      // aggiorna i movimenti della tabella Quote
        void DeleteRecordQuoteTab(int idQuote);                         // elimina una scrittura dal database

        QuoteTab GetLastQuoteTab();                                     // Prendo l'ultimo record della tabella perchè nel caso di giroconto devo conoscere il nuovo ID

        void InsertAccountMovement(ContoCorrente contoCorrente);        // inserisco il movimento nella tabella conto_corrente
        ContoCorrenteList GetContoCorrenteList();                       // Prendo tutti i record della tabella ContoCorrente
        ContoCorrenteList GetContoCorrenteByIdQuote(int idQuote);       // Il movimento del conto legato al giroconto selezionato
        ContoCorrenteList GetContoCorrenteByMovement(int idMovimento);  // Tutti i movimenti di giroconto
        void UpdateContoCorrenteByIdQuote(ContoCorrente contoCorrente);          // Aggiorno i movimenti della tabella ContoCorrente
        void DeleteAccount(int idCC);                                            // Elimino il movimento dalla tabella ContoCorrente
    }
}
