using FinanceManager.Events;
using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class ReportPorfitLossAnnoGestioniViewModel : ViewModelBase
    {
        ReportProfitLossList reportProfitLosses;
        public ReportPorfitLossAnnoGestioniViewModel(ReportProfitLossList reportProfitLossList, bool isDetailed)
        {
            reportProfitLosses = reportProfitLossList ?? throw new ArgumentNullException("Mancano i dati per la costruzione del report.");
            //AddTotals(reportProfitLosses);
            ProfitLossData = reportProfitLossList;
            IsDetailed = isDetailed == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public ReportProfitLossList ProfitLossData
        {
            get { return GetValue(() => ProfitLossData); }
            private set { SetValue(() => ProfitLossData, value); }
        }

        public System.Windows.Visibility IsDetailed
        {
            get { return GetValue(() => IsDetailed); }
            private set { SetValue(() => IsDetailed, value); }
        }
    }
}
