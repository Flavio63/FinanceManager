using FinanceManager.Services;
using FinanceManager.ViewModels;
using FinanceManager.Views;
using System.Windows;

namespace FinanceManager
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region dichiarazioni
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;
        private IManagerReportServices _managerReportServices;

        RegistryOwnerViewModel ownerViewModel;
        RegistryOwnerView ownerView;
        RegistryShareTypeViewModel registryShareTypeViewModel;
        RegistryShareTypeView shareTypeView;
        RegistryCurrencyViewModel registryCurrencyViewModel;
        RegistryCurrencyView currencyView;
        RegistryLocationViewModel registryLocationViewModel;
        RegistryLocationView locationView;
        RegistryFirmViewModel registryFirmViewModel;
        RegistryFirmView firmView;
        RegistryShareViewModel registryShareViewModel;
        RegistryShareView shareView;
        RegistryMovementTypeViewModel registryMovementTypeViewModel;
        RegistryMovementTypeView registryMovementTypeView;
        ManagerPortfolioMovementViewModel managerPortfolioMovementViewModel;
        ManagerPortfolioMovementView managerPortfolioMovementView;
        ManagerPortfolioChangeCurrencyViewModel managerPortfolioChangeCurrencyViewModel;
        ManagerPortfolioChangeCurrencyView managerPortfolioChangeCurrencyView;
        ManagerPortfolioSharesMovementViewModel managerPortfolioSharesMovementViewModel;
        ManagerPortfolioSharesMovementView managerPortfolioSharesMovementView;
        ManagerReportsViewModel managerReportsViewModel;
        ManagerReportsView managerReportsView;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            _registryServices = new RegistryService();
            _managerLiquidServices = new ManagerLiquidAssetServices();
            _managerReportServices = new ManagerReportServices();
        }

        private void OnClickGestioni(object sender, RoutedEventArgs e)
        {
            if (ownerView == null || !mainGrid.Children.Contains(ownerView))
            {
                ownerViewModel = new RegistryOwnerViewModel(_registryServices);
                ownerView = new RegistryOwnerView(ownerViewModel);
                mainGrid.Children.Add(ownerView);
            }
            else
            {
                mainGrid.Children.Remove(ownerView);
                ownerView = null;
                ownerViewModel = null;
            }
        }

        private void OnClickTipoTitoli(object sender, RoutedEventArgs e)
        {
            if (shareTypeView == null || !mainGrid.Children.Contains(shareTypeView))
            {
                registryShareTypeViewModel = new RegistryShareTypeViewModel(_registryServices);
                shareTypeView = new RegistryShareTypeView(registryShareTypeViewModel);
                mainGrid.Children.Add(shareTypeView);
            }
            else
            {
                mainGrid.Children.Remove(shareTypeView);
                shareTypeView = null;
                registryShareTypeViewModel = null;
            }
        }

        private void OnClickCurrency(object sender, RoutedEventArgs e)
        {
            if (currencyView == null || !mainGrid.Children.Contains(currencyView))
            {
                registryCurrencyViewModel = new RegistryCurrencyViewModel(_registryServices);
                currencyView = new RegistryCurrencyView(registryCurrencyViewModel);
                mainGrid.Children.Add(currencyView);
            }
            else
            {
                mainGrid.Children.Remove(currencyView);
                currencyView = null;
                registryCurrencyViewModel = null;
            }
        }

        private void OnClickLocation(object sender, RoutedEventArgs e)
        {
            if (locationView == null || !mainGrid.Children.Contains(locationView))
            {
                registryLocationViewModel = new RegistryLocationViewModel(_registryServices);
                locationView = new RegistryLocationView(registryLocationViewModel);
                mainGrid.Children.Add(locationView);
            }
            else
            {
                mainGrid.Children.Remove(locationView);
                locationView = null;
                registryLocationViewModel = null;
            }
        }

        private void OnClickFirm(object sender, RoutedEventArgs e)
        {
            if (firmView == null || !mainGrid.Children.Contains(firmView))
            {
                registryFirmViewModel = new RegistryFirmViewModel(_registryServices);
                firmView = new RegistryFirmView(registryFirmViewModel);
                mainGrid.Children.Add(firmView);
            }
            else
            {
                mainGrid.Children.Remove(firmView);
                firmView = null;
                registryFirmViewModel = null;
            }
        }

        private void OnClickShare(object sender, RoutedEventArgs e)
        {
            if (shareView == null || !mainGrid.Children.Contains(shareView))
            {
                registryShareViewModel = new RegistryShareViewModel(_registryServices);
                shareView = new RegistryShareView(registryShareViewModel);
                mainGrid.Children.Add(shareView);
            }
            else
            {
                mainGrid.Children.Remove(shareView);
                shareView = null;
                registryShareViewModel = null;
            }
        }

        private void OnClickMovementType(object sender, RoutedEventArgs e)
        {
            if (registryMovementTypeView == null || !mainGrid.Children.Contains(registryMovementTypeView))
            {
                registryMovementTypeViewModel = new RegistryMovementTypeViewModel(_registryServices);
                registryMovementTypeView = new RegistryMovementTypeView(registryMovementTypeViewModel);
                mainGrid.Children.Add(registryMovementTypeView);
            }
            else
            {
                mainGrid.Children.Remove(registryMovementTypeView);
                registryMovementTypeView = null;
                registryMovementTypeViewModel = null;
            }
        }

        private void OnClickManagerPorfolioMovement(object sender, RoutedEventArgs e)
        {
            if (managerPortfolioMovementView == null || !mainGrid.Children.Contains(managerPortfolioMovementView))
            {
                managerPortfolioMovementViewModel = new ManagerPortfolioMovementViewModel(_registryServices, _managerLiquidServices);
                managerPortfolioMovementView = new ManagerPortfolioMovementView(managerPortfolioMovementViewModel);
                mainGrid.Children.Add(managerPortfolioMovementView);
            }
            else
            {
                mainGrid.Children.Remove(managerPortfolioMovementView);
                managerPortfolioMovementView = null;
                managerPortfolioMovementViewModel = null;
            }
        }

        private void OnClickManagerChangeCurrency(object sender, RoutedEventArgs e)
        {
            if (managerPortfolioChangeCurrencyView == null || !mainGrid.Children.Contains(managerPortfolioChangeCurrencyView))
            {
                managerPortfolioChangeCurrencyViewModel = new ManagerPortfolioChangeCurrencyViewModel(_registryServices, _managerLiquidServices);
                managerPortfolioChangeCurrencyView = new ManagerPortfolioChangeCurrencyView(managerPortfolioChangeCurrencyViewModel);
                mainGrid.Children.Add(managerPortfolioChangeCurrencyView);
            }
            else
            {
                mainGrid.Children.Remove(managerPortfolioChangeCurrencyView);
                managerPortfolioChangeCurrencyView = null;
                managerPortfolioChangeCurrencyViewModel = null;
            }
        }

        private void OnClickManagerSharesMovement(object sender, RoutedEventArgs e)
        {
            if (managerPortfolioSharesMovementView == null || !mainGrid.Children.Contains(managerPortfolioSharesMovementView))
            {
                managerPortfolioSharesMovementViewModel = new ManagerPortfolioSharesMovementViewModel(_registryServices, _managerLiquidServices);
                managerPortfolioSharesMovementView = new ManagerPortfolioSharesMovementView(managerPortfolioSharesMovementViewModel);
                mainGrid.Children.Add(managerPortfolioSharesMovementView);
            }
            else
            {
                mainGrid.Children.Remove(managerPortfolioSharesMovementView);
                managerPortfolioSharesMovementView = null;
                managerPortfolioSharesMovementViewModel = null;
            }
        }

        private void OnClickManagerReports(object sender, RoutedEventArgs e)
        {
            if (managerReportsView == null || !mainGrid.Children.Contains(managerReportsView))
            {
                managerReportsViewModel = new ManagerReportsViewModel(_registryServices, _managerReportServices);
                managerReportsView = new ManagerReportsView(managerReportsViewModel);
                mainGrid.Children.Add(managerReportsView);
            }
            else
            {
                mainGrid.Children.Remove(managerReportsView);
                managerReportsView = null;
                managerReportsViewModel = null;
            }
        }
    }
}
