using FinanceManager.Events;
using FinanceManager.Models;

using System;

namespace FinanceManager.ViewModels
{
    public class TabControlSintesiViewModel : ViewModelBase
    {

        public TabControlSintesiViewModel(SintesiSoldiList sintesiSoldiTab)
        {
            SintesiSoldiTab = sintesiSoldiTab ?? throw new ArgumentNullException("Manager Liquid Asset Services in Tab Control Sintesi Soldi");
        }

        public SintesiSoldiList SintesiSoldiTab
        {
            get { return GetValue(() => SintesiSoldiTab); }
            private set { SetValue(() => SintesiSoldiTab, value); }
        }

    }
}
