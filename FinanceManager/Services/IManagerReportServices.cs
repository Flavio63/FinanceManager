using FinanceManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceManager.Services
{
    public interface IManagerReportServices
    {
        IList<int> GetAvailableYears();
        ReportProfitLossList GetReport1(IList<RegistryOwner> _selectedOwners, 
            IList<int> _selectedYears, bool isSynthetic = true);

        ReportMovementDetailedList GetMovementDetailed(int IdGestione, int IdTitolo);

        ReportTitoliAttiviList GetActiveAssets(IList<RegistryOwner> _selectedOwners, IList<int> _selectedAccount);

        AnalisiPortafoglio QuoteInvGeoSettori(IList<RegistryOwner> _selectedOwners);
    }
}
