using FinanceManager.Events;
using FinanceManager.Models;
using System.Windows.Data;

namespace FinanceManager.ViewModels
{
    public class ReportDeltaSplitMeseViewModel : ViewModelBase
    {
        public ReportDeltaSplitMeseViewModel(GuadagnoPerPeriodoList guadagnoPerPeriodos)
        {
            DataDeltaPerMonth = (CollectionView)CollectionViewSource.GetDefaultView(guadagnoPerPeriodos);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Gestione");
            DataDeltaPerMonth.GroupDescriptions.Add(groupDescription);
        }

        public CollectionView DataDeltaPerMonth 
        { 
            get { return GetValue(() => DataDeltaPerMonth); }
            private set { SetValue(() => DataDeltaPerMonth, value); }
        }

    }
}
