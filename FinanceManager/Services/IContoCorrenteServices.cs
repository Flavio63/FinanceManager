using FinanceManager.Models.Enumeratori;
using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public interface IContoCorrenteServices
    {
        /// <summary>
        /// Restituisce un record del conto corrente
        /// </summary>
        /// <param name="idContoCorrente">Id del record</param>
        /// <returns>Record Conto Corrente</returns>
        ContoCorrente GetContoCorrenteByIdCC(int idContoCorrente);
        /// <summary>
        /// Estrae tutti i record della tabella ContoCorrente
        /// </summary>
        /// <returns>Lista di ContoCorrente</returns>
        ContoCorrenteList GetContoCorrenteList();

        /// <summary>
        /// Estrae tutti i record con codice 1 e 2
        /// (deposito/prelievo) del conto 0 (zero)
        /// </summary>
        /// <returns>Lista di ContoCorrente</returns>
        ContoCorrenteList GetCCListByInvestmentSituation();

        /// <summary>
        /// Estrazione dei 2 record coinvolti nel giroconto interno o
        /// nel cambio valuta.
        /// </summary>
        /// <param name="modified">DateTime</param>
        /// <returns>List ContoCorrente</returns>
        ContoCorrenteList Get2ContoCorrentes(DateTime modified);

        ContoCorrenteList GetContoCorrenteByIdPortafoglio(int idPortafoglioTitoli); // I max 2 movimenti di ContoCorrente associati al trade

        /// <summary>
        /// Preleva dal conto zero la situazione degli investimenti per socio
        /// facendo la somma fra versati, investiti, disinvestiti e prelevati
        /// </summary>
        /// <returns>Situazione per socio</returns>
        InvestmentSituationList GetInvestmentSituation();

        /// <summary>
        /// Aggiorno la tabella conto_corrente
        /// </summary>
        /// <param name="contoCorrente">Il record con i dati aggiornati</param>
        /// <param name="idTipo">la scelta dell'identificativo</param>
        void UpdateRecordContoCorrente(ContoCorrente contoCorrente, TipologiaIDContoCorrente idTipo);
        /// <summary>
        /// Ritorna l'ultimo record inserito
        /// </summary>
        /// <returns>ContoCorrente</returns>
        ContoCorrente GetLastContoCorrente();
        /// <summary>
        /// Elimino il movimento dalla tabella ContoCorrente sulla base del conto titoli
        /// </summary>
        /// <param name="idContoTitoli"></param>
        void DeleteContoCorrenteByIdPortafoglioTitoli(int idContoTitoli);
        /// <summary>
        /// Elimina un record dalla tabella ContoCorrente
        /// sulla base di un id di riga
        /// </summary>
        /// <param name="idCC">id del record da eliminare</param>
        void DeleteRecordContoCorrente(int idCC);
        /// <summary>
        /// Restituisco le somme dei soldi presenti nei conti correnti
        /// suddivisi per gestione, valuta e tipo soldi
        /// </summary>
        /// <param name="IdConto"></param>
        /// <param name="IdGestione"></param>
        /// <param name="IdValuta"></param>
        /// <param name="IdTipoSoldi"></param>
        /// <returns>ContoCorrenteList</returns>
        ContoCorrenteList GetTotalAmountByAccount(int IdConto, int IdGestione = 0, int IdSocio = 0, int IdTipoSoldi = 0, int IdValuta = 0);

        /// <summary>
        /// Scrive un nuovo record per il conto corrente
        /// </summary>
        /// <param name="contoCorrente"></param>
        /// <exception cref="Exception"></exception>
        void InsertAccountMovement(ContoCorrente contoCorrente);
        
    }
}
