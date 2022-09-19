using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public interface IQuoteServices
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
        /// <param name="TipoSoldi">Tipologia dei soldi</param>
        /// <returns>Last id record inserted</returns>
        int Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi);

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

    }
}
