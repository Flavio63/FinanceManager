using FinanceManager.Events;
using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class ReportMovementDetailedViewModel : ViewModelBase
    {
        public ReportMovementDetailedViewModel(ReportMovementDetailedList reportMovementDetaileds)
        {
            MovementShareData = reportMovementDetaileds;
        }

        public ReportMovementDetailedList MovementShareData
        {
            get { return GetValue(() => MovementShareData); }
            private set { SetValue(() => MovementShareData, value); }
        }
    }
}
