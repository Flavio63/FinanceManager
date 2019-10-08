﻿using FinanceManager.Models;
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
        /// <param name="sintetico">True per sintesi e False per dettaglio</param>
        /// <returns>Observable Collection</returns>
        GuadagnoPerQuoteList GetQuoteGuadagno(bool sintetico);

        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        List<int> GetAnniFromGuadagni();

        /// <summary>
        /// Estraggo la data dalla tabella investimenti sulla base della
        /// nuova data di movimento (versamento / prelevamento)
        /// </summary>
        /// <param name="NuovaData">DateTime</params>
        /// <returns>DateTime</returns>
        DateTime GetDataPrecedente(DateTime NuovaData);
        /// <summary>
        /// Modifico la tabella quote_periodi cercando la data di inizio
        /// e modificando la data di fine
        /// </summary>
        /// <param name="DataDal">Data da cercare</param>
        /// <param name="DataAL">Data da modificare</param>
        void UpdateDataFine(DateTime DataDal, DateTime DataAL);
        /// <summary>
        /// Inserisco nella tabella quote_periodi la nuova coppia di date
        /// </summary>
        /// <param name="DataDal">La data di inizio periodo</param>
        void InsertPeriodoValiditaQuote(DateTime DataDal);

        /// <summary>
        /// Calcolo le nuove quote e le inserisco nella tabella quote_guadagno
        /// </summary>
        void ComputesAndInsertQuoteGuadagno();

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
        void UpdateRecordQuote_Guadagno(QuotePerPeriodo record_quote_guadagno);
        void DeleteRecordQuote_Guadagno(int id_quota);
    }
}
