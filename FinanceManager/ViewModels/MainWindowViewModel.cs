using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.Views;
using System.Reflection;
using System.Deployment.Application;
using System.Windows.Controls;
using System.Windows.Input;
using FinanceManager.Services.SQL;

namespace FinanceManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region dichiarazioni
        private readonly IRegistryServices _registryServices;
        private readonly IManagerLiquidAssetServices _managerLiquidServices;
        private readonly IManagerReportServices _managerReportServices;
        private readonly IDAFconnection _DafConnection;

        public ICommand OnClickOpenGestioni { get; set; }
        public ICommand OnClickOpenConti { get; set; }
        public ICommand OnClickOpenAziende { get; set; }
        public ICommand OnClickOpenTipologiaTitoli { get; set; }
        public ICommand OnClickOpenValute { get; set; }
        public ICommand OnClickOpenMovimenti { get; set; }
        public ICommand OnClickOpenQuoteInvestitori { get; set; }
        public ICommand OnClickOpenContoCorrente { get; set; }
        public ICommand OnClickPortafoglioTitoli { get; set; }
        public ICommand OnClickManagerReports { get; set; }

        public ICommand OnClickOpenSchedaTitoli { get; set; }

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
        RegistryMovementTypeViewModel registryMovementTypeViewModel;
        RegistryMovementTypeView registryMovementTypeView;

        SchedeTitoliViewModel SchedeTitoliModel;
        SchedeTitoliView SchedeTitoli;

        GestioneContoCorrenteView gestioneContoCorrenteView;
        GestioneContoCorrenteViewModel gestioneContoCorrenteViewModel;
        ManagerReportsViewModel managerReportsViewModel;
        ManagerReportsView managerReportsView;

        GestioneQuoteInvestitoriViewModel gestioneQuoteInvestitoriViewModel;
        GestioneQuoteInvestitoriView gestioneQuoteInvestitoriView;
        AcquistoVenditaTitoliViewModel acquistoVenditaTitoliViewModel;
        AcquistoVenditaTitoliView acquistoVenditaTitoliView;
        #endregion

        public MainWindowViewModel()
        {
            string versione = "";
            try
            {
                versione = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                Assembly.GetExecutingAssembly().GetName().Version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                versione = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            Titolo = string.Format("DAF-C Gestione Finanza ({0})", versione);
            _DafConnection = new DAFconnection();
            _registryServices = new RegistryService(_DafConnection);
            _managerLiquidServices = new ManagerLiquidAssetServices(_DafConnection);
            _managerReportServices = new ManagerReportServices(_DafConnection);

            OnClickOpenGestioni = new CommandHandler(OpenGestioni);
            OnClickOpenConti = new CommandHandler(OpenConti);
            OnClickOpenAziende = new CommandHandler(OpenAziende);
            OnClickOpenTipologiaTitoli = new CommandHandler(OpenTipologiaTitoli);
            OnClickOpenValute = new CommandHandler(OpenValute);
            OnClickOpenSchedaTitoli = new CommandHandler(OpenSchedaTitoli);
            OnClickOpenMovimenti = new CommandHandler(OpenMovimenti);
            OnClickOpenQuoteInvestitori = new CommandHandler(OpenQuoteInvestitori);
            OnClickPortafoglioTitoli = new CommandHandler(PortafoglioTitoli);
            OnClickOpenContoCorrente = new CommandHandler(OpenContoCorrente);
            OnClickManagerReports = new CommandHandler(OpenReports);
        }

        #region Anagrafica
        private void OpenGestioni(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (ownerView == null || !mainGrid.Children.Contains(shareTypeView))
            {
                ownerViewModel = new RegistryOwnerViewModel(_registryServices);
                ownerView = new RegistryOwnerView(ownerViewModel);
                mainGrid.Children.Add(ownerView);
            }
            else
            {
                ownerView = null;
                ownerViewModel = null;
            }
        }

        private void OpenTipologiaTitoli(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        private void OpenValute(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        private void OpenConti(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        private void OpenAziende(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        private void OpenMovimenti(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        private void OpenSchedaTitoli(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (SchedeTitoli == null || !mainGrid.Children.Contains(SchedeTitoli))
            {
                SchedeTitoliModel = new SchedeTitoliViewModel(_registryServices);
                SchedeTitoli = new SchedeTitoliView(SchedeTitoliModel);
                mainGrid.Children.Add(SchedeTitoli);
            }
            else
            {
                mainGrid.Children.Remove(SchedeTitoli);
                SchedeTitoli = null;
                SchedeTitoliModel = null;
            }
        }

        #endregion Anagrafica

        #region Gestionale
        private void PortafoglioTitoli(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (acquistoVenditaTitoliView == null || !mainGrid.Children.Contains(acquistoVenditaTitoliView))
            {
                acquistoVenditaTitoliViewModel = new AcquistoVenditaTitoliViewModel(_registryServices, _managerLiquidServices);
                acquistoVenditaTitoliView = new AcquistoVenditaTitoliView(acquistoVenditaTitoliViewModel);
                mainGrid.Children.Add(acquistoVenditaTitoliView);
            }
            else
            {
                mainGrid.Children.Remove(acquistoVenditaTitoliView);
                acquistoVenditaTitoliView = null;
                acquistoVenditaTitoliViewModel = null;
            }
        }

        private void OpenQuoteInvestitori(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (gestioneQuoteInvestitoriView == null || !mainGrid.Children.Contains(gestioneQuoteInvestitoriView))
            {
                gestioneQuoteInvestitoriViewModel = new GestioneQuoteInvestitoriViewModel(_registryServices, _managerLiquidServices);
                gestioneQuoteInvestitoriView = new GestioneQuoteInvestitoriView(gestioneQuoteInvestitoriViewModel);
                mainGrid.Children.Add(gestioneQuoteInvestitoriView);
            }
            else
            {
                mainGrid.Children.Remove(gestioneQuoteInvestitoriView);
                gestioneQuoteInvestitoriView = null;
                gestioneQuoteInvestitoriViewModel = null;
            }
        }

        private void OpenContoCorrente(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (gestioneContoCorrenteView == null || !mainGrid.Children.Contains(gestioneContoCorrenteView))
            {
                gestioneContoCorrenteViewModel = new GestioneContoCorrenteViewModel(_registryServices, _managerLiquidServices);
                gestioneContoCorrenteView = new GestioneContoCorrenteView(gestioneContoCorrenteViewModel);
                mainGrid.Children.Add(gestioneContoCorrenteView);
            }
            else
            {
                mainGrid.Children.Remove(gestioneContoCorrenteView);
                gestioneContoCorrenteView = null;
                gestioneContoCorrenteViewModel = null;
            }
        }

        #endregion Gestionale

        #region Reports

        private void OpenReports(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        #endregion Reports

        #region Versioni
        public string Titolo
        {
            get { return GetValue(() => Titolo); }
            set { SetValue(() => Titolo, value); }
        }
        #endregion

        public void IsChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _DafConnection.SetConnectionType(((RadioButton)sender).Name);
        }
    }
}
