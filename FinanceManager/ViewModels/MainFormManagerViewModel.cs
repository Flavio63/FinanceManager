using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class MainFormManagerViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }

        public MainFormManagerViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpData();
        }

        private void SetUpData()
        {
            GestioniScelte = new Collection<int>();
            ContiScelti = new Collection<int>();
            TipoMovimentoScelto = new Collection<int>();

            try
            {
                ListaGestioni = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
                MenuGestioni = new ObservableCollection<MenuItem>();
                foreach (RegistryOwner registryOwner in ListaGestioni)
                {
                    if (registryOwner.IdOwner > 0)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = registryOwner.OwnerName;
                        menuItem.Name = "Gestioni_" + registryOwner.IdOwner;
                        menuItem.IsCheckable = true;
                        menuItem.FontWeight = new System.Windows.FontWeight();
                        menuItem.FontSize = 12;
                        menuItem.AddHandler(MenuItem.CheckedEvent, new RoutedEventHandler(ItemChecked));
                        menuItem.AddHandler(MenuItem.UncheckedEvent, new RoutedEventHandler(ItemUnchecked));
                        MenuGestioni.Add(menuItem);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "MenuGestioni", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                ListaConti = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
                MenuConti = new ObservableCollection<MenuItem>();
                foreach (RegistryLocation registryLocation in ListaConti)
                {
                    if (registryLocation.IdLocation > 0)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = registryLocation.DescLocation;
                        menuItem.Name = "Conti_" + registryLocation.IdLocation;
                        menuItem.IsCheckable = true;
                        menuItem.FontWeight = new System.Windows.FontWeight();
                        menuItem.FontSize = 12;
                        menuItem.AddHandler(MenuItem.CheckedEvent, new RoutedEventHandler(ItemChecked));
                        menuItem.AddHandler(MenuItem.UncheckedEvent, new RoutedEventHandler(ItemUnchecked));
                        MenuConti.Add(menuItem);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "MenuConti", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                ListaTipoMovimenti = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
                MenuTipoMovimenti = new ObservableCollection<MenuItem>();
                foreach (RegistryMovementType registryMovementType in ListaTipoMovimenti)
                {
                    if (registryMovementType.IdMovement > 0)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = registryMovementType.DescMovement;
                        menuItem.Name = "Cosa_" + registryMovementType.IdMovement;
                        menuItem.IsCheckable = true;
                        menuItem.FontWeight = new System.Windows.FontWeight();
                        menuItem.FontSize = 12;
                        menuItem.AddHandler(MenuItem.CheckedEvent, new RoutedEventHandler(ItemChecked));
                        menuItem.AddHandler(MenuItem.UncheckedEvent, new RoutedEventHandler(ItemUnchecked));
                        MenuTipoMovimenti.Add(menuItem);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "MenuTipoMovimenti", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region setter & getter

        public ObservableCollection<RegistryOwner> ListaGestioni
        {
            get { return GetValue(() => ListaGestioni); }
            set { SetValue(() => ListaGestioni, value); }
        }
        public ObservableCollection<MenuItem> MenuGestioni
        {
            get { return GetValue(() => MenuGestioni); }
            set { SetValue(() => MenuGestioni, value); }
        }

        public ObservableCollection<RegistryLocation> ListaConti
        {
            get { return GetValue(() => ListaConti); ; }
            set { SetValue(() => ListaConti, value); }
        }
        public ObservableCollection<MenuItem> MenuConti
        {
            get { return GetValue(() => MenuConti); }
            set { SetValue(() => MenuConti, value); }
        }

        public ObservableCollection<RegistryMovementType> ListaTipoMovimenti
        {
            get { return GetValue(() => ListaTipoMovimenti); ; }
            set { SetValue(() => ListaTipoMovimenti, value); }
        }
        public ObservableCollection<MenuItem> MenuTipoMovimenti
        {
            get { return GetValue(() => MenuTipoMovimenti); }
            set { SetValue(() => MenuTipoMovimenti, value); }
        }

        public Collection<int> GestioniScelte
        {
            get { return GetValue(() => GestioniScelte); }
            set { SetValue(() => GestioniScelte, value); }
        }
        public Collection<int> ContiScelti
        {
            get { return GetValue(() => ContiScelti); }
            set { SetValue(() => ContiScelti, value); }
        }
        public Collection<int> TipoMovimentoScelto
        {
            get { return GetValue(() => TipoMovimentoScelto); }
            set { SetValue(() => TipoMovimentoScelto, value); }
        }
        #endregion

        #region event

        public void ItemChecked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            string menuTopLevel = menuItem.Name.Substring(0, menuItem.Name.IndexOf("_"));
            switch (menuTopLevel)
            {
                case ("Gestioni"):
                    GestioniScelte.Add(Convert.ToInt16(menuItem.Name.Substring(menuItem.Name.IndexOf("_") + 1)));
                    break;
                case ("Conti"):
                    ContiScelti.Add(Convert.ToInt16(menuItem.Name.Substring(menuItem.Name.IndexOf("_") + 1)));
                    break;
                case ("Cosa"):
                    TipoMovimentoScelto.Clear();
                    foreach (MenuItem menuItemMovimento in MenuTipoMovimenti)
                        menuItemMovimento.IsChecked = menuItemMovimento.Header != menuItem.Header ? false : true;
                    TipoMovimentoScelto.Add(Convert.ToInt16(menuItem.Name.Substring(menuItem.Name.IndexOf("_") + 1)));
                    break;
            }
        }
        public void ItemUnchecked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            string menuTopLevel = menuItem.Name.Substring(0, menuItem.Name.IndexOf("_"));
            switch (menuTopLevel)
            {
                case ("Gestioni"):
                    GestioniScelte.Remove(Convert.ToInt16(menuItem.Name.Substring(menuItem.Name.IndexOf("_") + 1)));
                    break;
                case ("Conti"):
                    ContiScelti.Remove(Convert.ToInt16(menuItem.Name.Substring(menuItem.Name.IndexOf("_") + 1))); break;
                case ("Cosa"):
                    TipoMovimentoScelto.Clear();
                    break;
            }
        }

        #endregion

        #region command
        public void CloseMe(object param)
        {
            MainFormManagerView MFMV = param as MainFormManagerView;
            DockPanel wp = MFMV.Parent as DockPanel;
            wp.Children.Remove(MFMV);
        }
        #endregion
    }
}
