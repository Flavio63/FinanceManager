using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class ReportGuadagniAnnoViewModel : ViewModelBase
    {

        public ReportGuadagniAnnoViewModel()
        {
            Desc_Anno = "2018";
        }

        public string Desc_Anno
        {
            get { return GetValue(() => Desc_Anno); }
            set { SetValue(() => Desc_Anno, value); }
        }
    }
}
