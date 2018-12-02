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

        QuoteList GetQuote();   // Calcolo le quote per investitore
        InvestitoreList GetInvestitori();

        QuoteTabList GetQuoteTab();                                 //Prendo tutti i record della tabella Quote
        void InsertInvestment(QuoteTab ActualQuote);                // Inserisce nuovo movimento nella tabella anche il movimento 12
        void UpdateQuoteTab(QuoteTab ActualQuote);                  // aggiorna i movimenti della tabella Quote (al momento chiamato solo per 1 e 2)
        void DeleteRecordQuoteTab(int idQuote);                     // elimina una scrittura dal database (al momento chiamato solo per 1 e 2)

        QuoteTab GetLastQuoteTab();                                 // Prendo l'ultimo record della tabella perchè nel caso di giroconto devo conoscere il nuovo ID

        void InsertAccountMovement(ContoCorrente contoCorrente);    // inserisco il movimento nella tabella conto_corrente
        ContoCorrenteList GetContoCorrenteList();                   // Prendo tutti i record della tabella ContoCorrente
        ContoCorrenteList GetContoCorrenteByMovement(int idMovimento);  

        //void AddGiroconto(); // coinvolge anche InsertAccountMovement questo è il movimento 12
        //void UpdateGiroconto(int idQuote);     // coinvolge anche il conco corrente da fare
    }
}
