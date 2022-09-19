using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using System;
using System.Collections.Generic;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerAndLocation(int IdOwner = 0, int idLocation = 0);
        PortafoglioTitoliList GetManagerLiquidAssetListByLinkMovimenti(DateTime link_movimenti);
        PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli);
        Ptf_CCList GetShareActiveAndAccountMovement(int id_gestione, int id_conto, int id_titolo);
        SintesiSoldiList GetCurrencyAvailable(int IdGestione = 0, int IdConto = 0, int IdValuta = 0);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint id_titolo);
        /// <summary>
        /// Prelevo le info per i costi medi dei titoli attivi
        /// </summary>
        /// <returns></returns>
        PortafoglioTitoliList GetCostiMediPerTitolo();

        void AddManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void UpdateManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);
        /// <summary>
        /// Calcola la quota ultima base investimento attivo
        /// restituendo il totale immesso, prelevato, assegnato e disponibile
        /// </summary>
        /// <returns>ObservableCollection</returns>
//        QuoteInvList GetQuoteInv();   // Calcolo le quote per investitore sulla base dell'investito attivo
        /// <summary>
        /// Calcola le quote del guadagno per investitore
        /// suddividendola per ogni periodo di validà delle 
        /// quote basate sugli investimenti versati / prelevati
        /// </summary>
        /// <param name="tipoReport">se 0 genera estrama sintesi, se 1 sintesi, se 2 il dettaglio</param>
        /// <returns>Observable Collection</returns>
        GuadagnoPerQuoteList GetQuoteGuadagno(int sintetico);
        /// <summary>
        /// Estrae tutti i movimenti di un dato conto per una data gestione di un anno per una valuta
        /// e Costruisce il dato cumulato partendo dal primo giorno inserito nel database
        /// </summary>
        /// <param name="IdConto">E' il conto corrente</param>
        /// <param name="IdGestione">E' la gestione nel conto</param>
        /// <param name="AnnoSelezionato">l'anno di cui si vuole il dettaglio</param>
        /// <param name="IdValuta">la valuta</param>
        /// <returns></returns>
        MovimentiContoList GetMovimentiContoGestioneValuta(int IdConto, int IdGestione, int AnnoSelezionato, int IdValuta);

        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        List<int> GetAnniFromGuadagni();

        /// <summary>
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_tipoSoldi">Identifica chi sta modificando l'investimento</param>
        /// <returns>int</returns>
        int GetIdPeriodoQuote(DateTime dateTime, int Id_tipoSoldi);

        void UpdateGuadagniTotaleAnno(GuadagnoPerQuote RecordQuoteGuadagno);
        /// <summary>
        /// Prelevo tutti i record della tabella degli Investimenti
        /// </summary>
        /// <returns>Lista di tipo QuoteTabList</returns>
        QuoteTabList GetQuoteTab();
        /// <summary>
        /// Inserisco un nuovo movimento di capitale
        /// nella tabella degli investimenti
        /// </summary>
        /// <param name="ActualQuote"></param>
        void InsertInvestment(QuoteTab ActualQuote);
        /// <summary>
        /// Aggiorno i movienti della tabella investimenti
        /// </summary>
        /// <param name="ActualQuote"></param>
        void UpdateQuoteTab(QuoteTab ActualQuote);
        /// <summary>
        /// Elimina il record dalla tabella investimenti in
        /// base al suo identificativo
        /// </summary>
        /// <param name="Id_ActualQuote"></param>
        void DeleteRecordQuoteTab(int Id_ActualQuote);

        /// <summary>
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        void AddSingoloGuadagno(ContoCorrente RecordContoCorrente);
        /// <summary>
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        void ModifySingoloGuadagno(ContoCorrente RecordContoCorrente);

        QuotePerPeriodoList GetAllRecordQuote_Guadagno();
        void DeleteRecordGuadagno_Totale_anno(int id_quota);

        /// <summary>
        /// Trovo l'id del record da modificare
        /// </summary>
        /// <param name="ActualQuote">Il record con le modifiche</param>
        /// <returns>id_quoteTab</returns>
        int GetIdQuoteTab(QuoteTab ActualQuote);

        /// <summary>
        /// Estraggo la quantità di utile disponibile
        /// sulla base dell'anno e della gestione
        /// </summary>
        /// <param name="guadagnoQuote">Il record con i dati da verificare</param>
        /// <returns>Disponibilità di utili</returns>
        double VerifyDisponibilitaUtili(GuadagnoPerQuote guadagnoQuote);

        /// <summary>
        /// Registro il prelievo di utili
        /// </summary>
        /// <param name="guadagnoQuote">Il record da inserire</param>
        void InsertPrelievoUtili(GuadagnoPerQuote guadagnoQuote);

        /// <summary>
        /// Elimino una registrazione di prelievo utili
        /// </summary>
        /// <param name="guadagnoPerQuote"></param>
        void DeletePrelievoUtili(GuadagnoPerQuote guadagnoPerQuote);
        /// <summary>
        /// Restituisco la somma dei soldi disponibili nella tabella
        /// degli investimenti
        /// </summary>
        /// <param name="IdInvestitore"></param>
        /// <param name="IdValuta"></param>
        /// <returns>QuoteTabList</returns>
        QuoteTabList GetTotalAmountByCurrency(int IdInvestitore, int IdValuta = 0);
        /// <summary>
        /// Restituisco il record in base al suo id di riga
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        QuoteTab GetQuoteTabById(int Id);
    }
}
