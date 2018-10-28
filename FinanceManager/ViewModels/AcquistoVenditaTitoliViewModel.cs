using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class AcquistoVenditaTitoliViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }

        public AcquistoVenditaTitoliViewModel (IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpData();
        }

        private void SetUpData()
        {

        }

        #region Getter&Setter

        #endregion

        #region command
        public void CloseMe(object param)
        {
            AcquistoVenditaTitoliView MFMV = param as AcquistoVenditaTitoliView;
            Grid wp = MFMV.Parent as Grid;
            wp.Children.Remove(MFMV);
        }
        #endregion
    }
}
