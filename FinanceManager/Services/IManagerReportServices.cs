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
    }
}
