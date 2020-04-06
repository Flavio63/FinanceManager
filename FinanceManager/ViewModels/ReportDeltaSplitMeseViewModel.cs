using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FinanceManager.ViewModels
{
    public class ReportDeltaSplitMeseViewModel : ViewModelBase
    {
        IManagerReportServices _managerLiquidAssetServices;

        public ReportDeltaSplitMeseViewModel(IManagerReportServices managerReportServices, IList<RegistryOwner> _RegistryOwners, IList<int> SelectedYears, bool IsYear, bool IsAggregated)
        {
            _managerLiquidAssetServices = managerReportServices ?? throw new ArgumentNullException("Manca il servizio");
            DataDeltaPerMonth = (CollectionView)CollectionViewSource.GetDefaultView(_managerLiquidAssetServices.GetDeltaPeriod(_RegistryOwners, SelectedYears, IsYear, IsAggregated));
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Gestione");
            DataDeltaPerMonth.GroupDescriptions.Add(groupDescription);
            SetDataDock();
        }

        public CollectionView DataDeltaPerMonth 
        { 
            get { return GetValue(() => DataDeltaPerMonth); }
            private set { SetValue(() => DataDeltaPerMonth, value); }
        }

        private void SetDataDock()
        {
        }

    }
}
