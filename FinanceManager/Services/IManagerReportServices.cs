using System.Collections.Generic;

namespace FinanceManager.Services
{
    public interface IManagerReportServices
    {
        IList<int> GetAvailableYears();
    }
}
