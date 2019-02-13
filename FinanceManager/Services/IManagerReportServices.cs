using FinanceManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceManager.Services
{
    public interface IManagerReportServices
    {
        IList<int> GetAvailableYears();
        ReportProfitLossList GetReport1(IList<int> _selectedOwners, 
            IList<int> _selectedYears, IList<int> _selectedCurrency, double[] exchangeValue = null);
    }
}
