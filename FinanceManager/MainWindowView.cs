using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.ViewModels;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager
{
    public class MainWindowView : ViewModelBase
    {
        #region dichiarazioni
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;
        private IManagerReportServices _managerReportServices;

        public ICommand OnClickOpenGestioni { get; set; }
        public ICommand OnClickOpenConti { get; set; }
        public ICommand OnClickOpenAziende { get; set; }
        public ICommand OnClickOpenTitoli { get; set; }
        public ICommand OnClickOpenTipologiaTitoli { get; set; }
        public ICommand OnClickOpenValute { get; set; }
        public ICommand OnClickOpenMovimenti { get; set; }
        public ICommand OnClickOpenQuoteInvestitori { get; set; }
        public ICommand OnClickOpenContoCorrente { get; set; }
        public ICommand OnClickPortafoglioTitoli { get; set; }
        public ICommand OnClickManagerReports { get; set; }

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
        ManagerPortfolioSharesMovementViewModel managerPortfolioSharesMovementViewModel;
        ManagerPortfolioSharesMovementView managerPortfolioSharesMovementView;
        ManagerReportsViewModel managerReportsViewModel;
        ManagerReportsView managerReportsView;

        GestioneQuoteInvestitoriViewModel gestioneQuoteInvestitoriViewModel;
        GestioneQuoteInvestitoriView gestioneQuoteInvestitoriView;
        #endregion

        public MainWindowView()
        {
            _registryServices = new RegistryService();
            _managerLiquidServices = new ManagerLiquidAssetServices();
            _managerReportServices = new ManagerReportServices();

            OnClickOpenGestioni = new CommandHandler(OpenGestioni);
            OnClickOpenConti = new CommandHandler(OpenConti);
            OnClickOpenAziende = new CommandHandler(OpenAziende);
            OnClickOpenTitoli = new CommandHandler(OpenTitoli);
            OnClickOpenTipologiaTitoli = new CommandHandler(OpenTipologiaTitoli);
            OnClickOpenValute = new CommandHandler(OpenValute);
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

        private void OpenTitoli(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        #endregion Anagrafica

        #region Gestionale
        private void PortafoglioTitoli(object param)
        {
            DockPanel mainGrid = param as DockPanel;
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

        #endregion Gestionale

        #region Reports

        private void OpenReports(object param)
        {
            DockPanel mainGrid = param as DockPanel;
            if (managerReportsView == null || !mainGrid.Children.Contains(managerReportsView))
            {
                managerReportsViewModel = new ManagerReportsViewModel(_registryServices, _managerReportServices);
                managerReportsView = new ManagerReportsView(managerReportsViewModel);
            }
            else
            {
                mainGrid.Children.Remove(managerReportsView);
                managerReportsView = null;
                managerReportsViewModel = null;
            }
        }

        #endregion Reports

        //private void OpenGestioni(object param)
        //{
        //    {
        //        mainFormManagerViewModel = new MainFormManagerViewModel(_registryServices, _managerLiquidServices);
        //        mainFormManager = new MainFormManagerView(mainFormManagerViewModel);
        //    }
        //    {
        //        mainFormManager = null;
        //    }
        //}

    }
}
