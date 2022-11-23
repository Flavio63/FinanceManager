using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public interface IQuoteGuadagniServices
    {
        /// <summary>
        /// Verifico se nella data di inserimento è già presente
        /// un investimento
        /// </summary>
        /// <param name="ActualCC">Il record per verificare la presenza di un gemello</param>
        /// <param name="Id_Tipo_Soldi">Il tipo soldi che si sta movimentando</param>
        /// <returns>-1 se falso altrimenti il numero del record</returns>
        object VerifyInvestmentDate(ContoCorrente ActualCC, int Id_Tipo_Soldi);

        /// <summary>
        /// Modifico la tabella quote_periodi modificando la data di fine
        /// e inserendo il nuovo record
        /// </summary>
        /// <param name="DataDal">Data da cercare</param>
        /// <param name="TipoSoldi">Tipo_Gestione dei soldi</param>
        /// <returns>Il record di quote_periodi</returns>
        QuotePeriodi Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi);

        /// <summary>
        /// Calcolo le nuove quote e le inserisco nella tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        /// <param name="NuovoPeriodo">Il nuovo periodo da inserire in tabella</param>
        void ComputesAndInsertQuoteGuadagno(int Tipo_Soldi, int NuovoPeriodo);

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
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_Gestione">Identifica il tipo di gestione da cui si deducono le quote</param>
        /// <returns>int</returns>
        int GetIdPeriodoQuote(DateTime dateTime, int Id_Gestione);
        /// <summary>
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        void AddSingoloGuadagno(ContoCorrente RecordContoCorrente);
        /// <summary>
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote delle perdite per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        void DeleteRecordGuadagno_Totale_anno(int id_quota);
        /// <summary>
        /// Tramite l'ultimo record conto_corrente modificate
        /// calcolo e modifico le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        void ModifySingoloGuadagno(ContoCorrente RecordContoCorrente);
        /// <summary>
        /// Tramite l'ultimo record conto_corrente modificato
        /// calcolo e modifico le quote delle perdite per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
    }
}
