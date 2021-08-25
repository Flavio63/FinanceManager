using FinanceManager.Events;
using FinanceManager.Models;

namespace FinanceManager.ViewModels
{
    public class ReportMovimentiContoViewModel : ViewModelBase
    {
        public ReportMovimentiContoViewModel(MovimentiContoList movimentiContoList)
        {
            MovimentiContoData = movimentiContoList;
        }

        public MovimentiContoList MovimentiContoData
        {
            get { return GetValue(() => MovimentiContoData); }
            private set { SetValue(() => MovimentiContoData, value); }
        }
    }
}
