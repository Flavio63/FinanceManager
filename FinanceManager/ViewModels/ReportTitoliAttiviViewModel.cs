using FinanceManager.Events;
using FinanceManager.Models;

namespace FinanceManager.ViewModels
{
    public class ReportTitoliAttiviViewModel : ViewModelBase
    {

        public ReportTitoliAttiviViewModel(ReportTitoliAttiviList reportTitoliAttiviList)
        {
            ActiveAssets = reportTitoliAttiviList;
        }

        public ReportTitoliAttiviList ActiveAssets
        {
            get { return GetValue(() => ActiveAssets); }
            private set { SetValue(() => ActiveAssets, value); }
        }
    }
}
