using FinanceManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceManager.Services
{
    public interface IManagerReportServices
    {
        IList<int> GetAvailableYears();
        ReportProfitLossList GetReport1(IList<RegistryGestioni> _selectedOwners, 
            IList<int> _selectedYears, bool isSynthetic = true);

        ReportMovementDetailedList GetMovementDetailed(int IdGestione, int IdTitolo);

        ReportTitoliAttiviList GetActiveAssets(IList<RegistryGestioni> _selectedOwners, IList<RegistryLocation> _selectedAccount);

        AnalisiPortafoglio QuoteInvGeoSettori(IList<RegistryGestioni> _selectedOwners);

        GuadagnoPerPeriodoList GetDeltaPeriod(IList<RegistryGestioni> _selectedOwners, IList<int> _selectedYears, bool isYear, bool isAggregated);

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

    }
}
