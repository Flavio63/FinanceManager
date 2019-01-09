using FinanceManager.Models;
using System.Data;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerAndLocation(int IdOwner = 0, int idLocation = 0);
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdMovement);
        PortafoglioTitoliList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndTitolo(int idGestione, int idConto, int idTitolo);
        PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli);
        Ptf_CCList GetShare_AccountMovement(int id_gestione, int id_conto, int id_titolo);
        SintesiSoldiList GetCurrencyAvailable(int IdGestione = 0, int IdConto = 0, int IdValuta = 0);
        PortafoglioTitoliList GetShareMovements(int IdOwner, int IdLocation, uint IdShare);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare);
        double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void UpdateManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);

        QuoteList GetQuote();   // Calcolo le quote per investitore
        InvestitoreList GetInvestitori();

        QuoteTabList GetQuoteTab();                                     //Prendo tutti i record della tabella AndQuote
        void InsertInvestment(QuoteTab ActualQuote);                    // Inserisce nuovo movimento nella tabella anche il movimento 12
        void UpdateQuoteTab(QuoteTab ActualQuote);                      // aggiorna i movimenti della tabella AndQuote
        void DeleteRecordQuoteTab(int idQuote);                         // elimina una scrittura dal database

        QuoteTab GetLastQuoteTab();                                     // Prendo l'ultimo record della tabella perchè nel caso di giroconto devo conoscere il nuovo ID

        void InsertAccountMovement(ContoCorrente contoCorrente);        // inserisco il movimento nella tabella conto_corrente
        ContoCorrenteList GetContoCorrenteList();                       // Prendo tutti i record della tabella ContoCorrente
        ContoCorrenteList GetContoCorrenteByIdQuote(int idQuote);       // Il movimento del conto legato al giroconto selezionato
        ContoCorrenteList GetContoCorrenteByMovement(int idMovimento);  // Tutti i movimenti di giroconto
        void UpdateContoCorrenteByIdCC(ContoCorrente contoCorrente);                // Aggiorno il movimento della tabella ContoCorrente sulla base dell'id del record
        void UpdateContoCorrenteByIdQuote(ContoCorrente contoCorrente);             // Aggiorno il movimento della tabella ContoCorrente sulla base della tabella quote investimento
        void UpdateContoCorrenteByIdPortafoglioTitoli(ContoCorrente contoCorrente); // Aggiorno il movimento della tabella ContoCorrente sulla base del conto titoli
        void DeleteContoCorrenteByIdPortafoglioTitoli(int idContoTitoli);           // Elimino il movimento dalla tabella ContoCorrente sulla base del conto titoli
        void DeleteAccount(int idCC);                                               // Elimino il movimento dalla tabella ContoCorrente
    }
}
