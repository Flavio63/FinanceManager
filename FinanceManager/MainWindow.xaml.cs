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
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;

        public MainWindow()
        {
            InitializeComponent();
            _registryServices = new RegistryService();
            _managerLiquidServices = new ManagerLiquidAssetServices();
        }

        private void OnClickGestioni(object sender, RoutedEventArgs e)
        {
            RegistryOwnerViewModel ownerViewModel = new RegistryOwnerViewModel(_registryServices);
            RegistryOwnerView ownerView = new RegistryOwnerView(ownerViewModel);
            mainGrid.Children.Add(ownerView);
        }

        private void OnClickTipoTitoli(object sender, RoutedEventArgs e)
        {
            RegistryShareTypeViewModel registryShareTypeViewModel = new RegistryShareTypeViewModel(_registryServices);
            RegistryShareTypeView shareTypeView = new RegistryShareTypeView(registryShareTypeViewModel);
            mainGrid.Children.Add(shareTypeView);
        }

        private void OnClickCurrency(object sender, RoutedEventArgs e)
        {
            RegistryCurrencyViewModel registryCurrencyViewModel = new RegistryCurrencyViewModel(_registryServices);
            RegistryCurrencyView currencyView = new RegistryCurrencyView(registryCurrencyViewModel);
            mainGrid.Children.Add(currencyView);
        }

        private void OnClickLocation(object sender, RoutedEventArgs e)
        {
            RegistryLocationViewModel registryLocationViewModel = new RegistryLocationViewModel(_registryServices);
            RegistryLocationView locationView = new RegistryLocationView(registryLocationViewModel);
            mainGrid.Children.Add(locationView);
        }

        private void OnClickMarket(object sender, RoutedEventArgs e)
        {
            RegistryMarketViewModel registryMarketViewModel = new RegistryMarketViewModel(_registryServices);
            RegistryMarketView marketView = new RegistryMarketView(registryMarketViewModel);
            mainGrid.Children.Add(marketView);
        }

        private void OnClickFirm(object sender, RoutedEventArgs e)
        {
            RegistryFirmViewModel registryFirmViewModel = new RegistryFirmViewModel(_registryServices);
            RegistryFirmView firmView = new RegistryFirmView(registryFirmViewModel);
            mainGrid.Children.Add(firmView);
        }

        private void OnClickShare(object sender, RoutedEventArgs e)
        {
            RegistryShareViewModel registryShareViewModel = new RegistryShareViewModel(_registryServices);
            RegistryShareView shareView = new RegistryShareView(registryShareViewModel);
            mainGrid.Children.Add(shareView);
        }

        private void OnClickMovementType(object sender, RoutedEventArgs e)
        {
            RegistryMovementTypeViewModel registryMovementTypeViewModel = new RegistryMovementTypeViewModel(_registryServices);
            RegistryMovementTypeView registryMovementTypeView = new RegistryMovementTypeView(registryMovementTypeViewModel);
            mainGrid.Children.Add(registryMovementTypeView);
        }

        private void OnClickManagerPorfolioMovement(object sender, RoutedEventArgs e)
        {
            ManagerPortfolioMovementViewModel managerPortfolioMovementViewModel = new ManagerPortfolioMovementViewModel(_registryServices, _managerLiquidServices);
            ManagerPortfolioMovementView managerPortfolioMovementView = new ManagerPortfolioMovementView(managerPortfolioMovementViewModel);
            mainGrid.Children.Add(managerPortfolioMovementView);
        }

        private void OnClickManagerChangeCurrency(object sender, RoutedEventArgs e)
        {
            ManagerPortfolioChangeCurrencyViewModel managerPortfolioChangeCurrencyViewModel = new ManagerPortfolioChangeCurrencyViewModel(_registryServices, _managerLiquidServices);
            ManagerPortfolioChangeCurrencyView managerPortfolioChangeCurrencyView = new ManagerPortfolioChangeCurrencyView(managerPortfolioChangeCurrencyViewModel);
            mainGrid.Children.Add(managerPortfolioChangeCurrencyView);
        }

        private void OnClickManagerSharesMovement(object sender, RoutedEventArgs e)
        {
            ManagerPortfolioSharesMovementViewModel managerPortfolioSharesMovementViewModel = new ManagerPortfolioSharesMovementViewModel(_registryServices, _managerLiquidServices);
            ManagerPortfolioSharesMovementView managerPortfolioSharesMovementView = new ManagerPortfolioSharesMovementView(managerPortfolioSharesMovementViewModel);
            mainGrid.Children.Add(managerPortfolioSharesMovementView);
        }
    }
}
