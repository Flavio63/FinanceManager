using FinanceManager.Models;
using FinanceManager.Models.Enum;
using System;
using System.Collections.Generic;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerAndLocation(int IdOwner = 0, int idLocation = 0);
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdMovement);
        PortafoglioTitoliList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndTitolo(int idGestione, int idConto, int idTitolo);
        PortafoglioTitoliList GetManagerLiquidAssetListByLinkMovimenti(DateTime link_movimenti);
        PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli);
        Ptf_CCList GetShareActiveAndAccountMovement(int id_gestione, int id_conto, int id_titolo);
        SintesiSoldiList GetCurrencyAvailable(int IdGestione = 0, int IdConto = 0, int IdValuta = 0);
        PortafoglioTitoliList GetShareMovements(int IdOwner, int IdLocation, uint id_titolo);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint id_titolo);
        double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void UpdateManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);

        QuoteInvList GetQuoteInv();   // Calcolo le quote per investitore sulla base dell'investito attivo
        /// <summary>
        /// Calcola le quote del guadagno per investitore
        /// suddividendola per ogni periodo di validà delle 
        /// quote basate sugli investimenti versati / prelevati
        /// </summary>
        /// <param name="tipoReport">se 0 genera estrama sintesi, se 1 sintesi, se 2 il dettaglio</param>
        /// <returns>Observable Collection</returns>
        GuadagnoPerQuoteList GetQuoteGuadagno(int sintetico);

        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        List<int> GetAnniFromGuadagni();

        /// <summary>
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_tipoSoldi">Identifica chi sta modificando l'investimento</param>
        /// <returns>int</returns>
        int GetIdPeriodoQuote(DateTime dateTime, int Id_tipoSoldi);

        /// <summary>
        /// Modifico la tabella quote_periodi modificando la data di fine
        /// e inserendo il nuovo record
        /// </summary>
        /// <param name="DataDal">Data da cercare</param>
        /// <param name="TipoSoldi">Tipologia dei soldi</param>
        /// <returns>Last id record inserted</returns>
        int Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi);

        /// <summary>
        /// Calcolo le nuove quote e le inserisco nella tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        void ComputesAndInsertQuoteGuadagno(int Tipo_Soldi);

        /// <summary>
        /// Calcolo le nuove quote e modifico la tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        void ComputesAndModifyQuoteGuadagno(int Tipo_Soldi);
        /// <summary>
        /// Aggiorno la tabella Guadagni_totale_anno con le nuove
        /// quote per il periodo interessato alle modifiche
        /// </summary>
        /// <param name="Id_Periodo_Quote">il periodo da modificare</param>
        /// <param name="Id_Tipo_Soldi">Il tipo soldi</param>
        void UpdateGuadagniTotaleAnno(int Id_Periodo_Quote, int Id_Tipo_Soldi);
        /// <summary>
        /// Aggiorno la tabella Guadagni_totale_anno nel caso di
        /// modifiche del record di prelievo utili
        /// </summary>
        /// <param name="RecordQuoteGuadagno">il record da modificare</param>
        void UpdateGuadagniTotaleAnno(GuadagnoPerQuote RecordQuoteGuadagno);

        /// <summary>
        /// Recupero l'ultimo id delle coppie di date inserite
        /// </summary>
        /// <returns>Identificativo</returns>
        int GetLastPeriodoValiditaQuote();

        QuoteTabList GetQuoteTab();                                     //Prendo tutti i record della tabella AndQuote
        void InsertInvestment(QuoteTab ActualQuote);                    // Inserisce nuovo movimento nella tabella anche il movimento 12

        /// <summary>
        /// Calcola il totale degli investimenti di
        /// un investitore (somma algebrica)
        /// </summary>
        /// <param name="IdGestione">Identificativo</param>
        /// <returns>double</returns>
        double GetInvestmentByIdGestione(int IdGestione);

        void UpdateQuoteTab(QuoteTab ActualQuote);                      // aggiorna i movimenti della tabella AndQuote
        void DeleteRecordQuoteTab(int idQuote);                         // elimina una scrittura dal database

        QuoteTab GetLastQuoteTab();                                     // Prendo l'ultimo record della tabella perchè nel caso di giroconto devo conoscere il nuovo ID

        void InsertAccountMovement(ContoCorrente contoCorrente);        // inserisco il movimento nella tabella conto_corrente
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

        ContoCorrenteList GetContoCorrenteList();                       // Prendo tutti i record della tabella ContoCorrente
        ContoCorrente GetContoCorrenteByIdCC(int idRecord);             // Prendo un solo record dalla tabella ContoCorrente
        ContoCorrente GetContoCorrenteByIdQuote(int idQuote);       // Il movimento del conto legato al giroconto selezionato
        ContoCorrenteList GetContoCorrenteByMovement(int idMovimento);  // Tutti i movimenti di giroconto
        ContoCorrenteList GetContoCorrenteByIdPortafoglio(int idPortafoglioTitoli); // I max 2 movimenti di ContoCorrente associati al trade
        /// <summary>
        /// Aggiorno la tabella conto_corrente
        /// </summary>
        /// <param name="contoCorrente">Il record con i dati aggiornati</param>
        /// <param name="idTipo">la scelta dell'identificativo</param>
        void UpdateRecordContoCorrente(ContoCorrente contoCorrente, TipologiaIDContoCorrente idTipo);

        void DeleteContoCorrenteByIdPortafoglioTitoli(int idContoTitoli);           // Elimino il movimento dalla tabella ContoCorrente sulla base del conto titoli
        /// <summary>
        /// Elimina un record dalla tabella ContoCorrente
        /// sulla base di un id di riga
        /// </summary>
        /// <param name="idCC">id del record da eliminare</param>
        void DeleteRecordContoCorrente(int idCC);
        QuotePerPeriodoList GetAllRecordQuote_Guadagno();
        void InsertRecordQuote_Guadagno(QuotePerPeriodo record_quote_guadagno);
        void DeleteRecordQuote_Guadagno(int id_quota);

        /// <summary>
        /// Verifico se nella data di inserimento è già presente
        /// un investimento
        /// </summary>
        /// <param name="ActualQuote">Il record per verificare</param>
        /// <param name="Id_Tipo_Soldi">Il tipo soldi che si sta movimentando</param>
        /// <returns>-1 se falso altrimenti il numero del periodo quote</returns>
        int VerifyInvestmentDate(QuoteTab ActualQuote, int Id_Tipo_Soldi);
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
        /// <param name="gudadagnoQuote">Il record con i dati da verificare</param>
        /// <returns>Disponibilità di utili</returns>
        double VerifyDisponibilitaUtili(GuadagnoPerQuote gudadagnoQuote);

        /// <summary>
        /// Registro il prelievo di utili
        /// </summary>
        /// <param name="gudadagnoQuote">Il record da inserire</param>
        void InsertPrelievoUtili(GuadagnoPerQuote gudadagnoQuote);

    }
}
