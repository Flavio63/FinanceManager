using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class MainFormManagerViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }
        AcquistoVenditaTitoliViewModel acquistoVenditaTitoliViewModel;
        AcquistoVenditaTitoliView acquistoVenditaTitoliView;

        public MainFormManagerViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpData();
        }

        private void SetUpData()
        {
            GestioniScelte = new Collection<RegistryOwner>();
            ContiScelti = new Collection<RegistryLocation>();
            TipoMovimentoScelto = new RegistryMovementType();

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

        public Collection<RegistryOwner> GestioniScelte
        {
            get { return GetValue(() => GestioniScelte); }
            set { SetValue(() => GestioniScelte, value); }
        }
        public Collection<RegistryLocation> ContiScelti
        {
            get { return GetValue(() => ContiScelti); }
            set { SetValue(() => ContiScelti, value); }
        }
        public RegistryMovementType TipoMovimentoScelto
        {
            get { return GetValue(() => TipoMovimentoScelto); }
            set { SetValue(() => TipoMovimentoScelto, value); }
        }
        #endregion

        #region event
        /// <summary>
        /// Quando viene selezionato una voce del menu si compila un array di codici per
        /// generare una query di interrogazione del database e riavere una risposta che
        /// compili la form
        /// </summary>
        /// <param name="sender">La voce di menu selezionata</param>
        /// <param name="e">evento di selezione</param>
        public void ItemChecked(object sender, RoutedEventArgs e)
        {
            MenuItem menuSubItem = e.Source as MenuItem;
            MenuItem menuTopLevel = ItemsControl.ItemsControlFromItemContainer(menuSubItem) as MenuItem;
            Grid mainGrid = ((Grid)((Menu)menuTopLevel.Parent).Parent).Children[2] as Grid;
            switch (menuTopLevel.Header)
            {
                case ("Cosa vuoi fare"):
                    foreach (MenuItem menuItemMovimento in MenuTipoMovimenti)
                    {
                        menuItemMovimento.IsChecked = menuItemMovimento.Header != menuSubItem.Header ? false : true;
                        menuItemMovimento.IsEnabled = menuItemMovimento.Header != menuSubItem.Header ? false : true;
                    }
                    TipoMovimentoScelto = _services.GetMovementType((Convert.ToInt16(menuSubItem.Name.Substring(menuSubItem.Name.IndexOf("_") + 1))));
                    break;
                case ("Gestioni"):
                    if (TipoMovimentoScelto.IdMovement != 12)
                    {
                        foreach (MenuItem menuGestioni in MenuGestioni)
                        {
                            menuGestioni.IsChecked = menuGestioni.Header != menuSubItem.Header ? false : true;
                            menuGestioni.IsEnabled = menuGestioni.Header != menuSubItem.Header ? false : true;
                        }
                    }
                    GestioniScelte.Add(_services.GetOwner(Convert.ToInt16(menuSubItem.Name.Substring(menuSubItem.Name.IndexOf("_") + 1))));
                    if (GestioniScelte.Count == 2)
                    {
                        foreach (MenuItem menuGestioni in MenuGestioni)
                        {
                            menuGestioni.IsEnabled = menuGestioni.IsChecked;
                        }
                    }
                    break;
                case ("Conti di appoggio"):
                    if (TipoMovimentoScelto.IdMovement != 3 || TipoMovimentoScelto.IdMovement != 12)
                    {
                        foreach (MenuItem menuConto in MenuConti)
                        {
                            menuConto.IsChecked = menuConto.Header != menuSubItem.Header ? false : true;
                            menuConto.IsEnabled = menuConto.Header != menuSubItem.Header ? false : true;
                        }
                    }
                    ContiScelti.Add(_services.GetLocation(Convert.ToInt16(menuSubItem.Name.Substring(menuSubItem.Name.IndexOf("_") + 1))));
                    if (TipoMovimentoScelto.IdMovement == 5 || TipoMovimentoScelto.IdMovement == 6)
                    {
                        acquistoVenditaTitoliViewModel =
                          new AcquistoVenditaTitoliViewModel(_services, _liquidAssetServices, GestioniScelte, ContiScelti, TipoMovimentoScelto);
                        acquistoVenditaTitoliView = new AcquistoVenditaTitoliView(acquistoVenditaTitoliViewModel);
                        mainGrid.Children.Add(acquistoVenditaTitoliView);
                        mainGrid.Visibility = Visibility.Visible;
                        mainGrid.IsVisibleChanged += MainGrid_IsVisibleChanged;
                    }
                    break;
            }
        }
        /// <summary>
        /// Quando si chiude la maschera resetta le scelte nel menu
        /// </summary>
        /// <param name="sender">La grid che viene chiusa</param>
        /// <param name="e">evento di cambio visibilità</param>
        private void MainGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = ((Grid)((Grid)sender).Parent).Children[2] as Grid;
            grid.Visibility = Visibility.Visible;
            Menu menu = ((Menu)((Grid)((Grid)sender).Parent).Children[0]);
            foreach (MenuItem menuItem in menu.Items)
                foreach (MenuItem subMenuItem in menuItem.Items)
                {
                    subMenuItem.IsChecked = false;
                    subMenuItem.IsEnabled = true;
                }
        }

        /// <summary>
        /// Quando viene deselezionata una voce di menu si toglie il 
        /// corrispettivo valore dall'array di ID e si rilancia la query
        /// </summary>
        /// <param name="sender">La voce di menu selezionata</param>
        /// <param name="e">evento di deselezione</param>
        public void ItemUnchecked(object sender, RoutedEventArgs e)
        {
            MenuItem menuSubItem = e.Source as MenuItem;
            MenuItem menuTopLevel = ItemsControl.ItemsControlFromItemContainer(menuSubItem) as MenuItem;
            Grid mainGrid = ((Grid)((Menu)menuTopLevel.Parent).Parent).Children[2] as Grid;
            switch (menuTopLevel.Header)
            {
                case ("Cosa vuoi fare"):
                    foreach (MenuItem menuItemMovimento in MenuTipoMovimenti)
                    {
                        menuItemMovimento.IsChecked = false;
                        menuItemMovimento.IsEnabled = true;
                    }
                    foreach (MenuItem menuItemGestioni in MenuGestioni)
                    {
                        menuItemGestioni.IsChecked = false;
                        menuItemGestioni.IsEnabled = true;
                    }
                    foreach (MenuItem menuItemConti in MenuConti)
                    {
                        menuItemConti.IsChecked = false;
                        menuItemConti.IsEnabled = true;
                    }
                    TipoMovimentoScelto = new RegistryMovementType();
                    GestioniScelte.Clear();
                    ContiScelti.Clear();
                    mainGrid.Children.Clear();
                    break;
                case ("Gestioni"):
                    foreach (RegistryOwner registryOwner in GestioniScelte)
                        if (registryOwner.IdOwner == Convert.ToInt16(menuSubItem.Name.Substring(menuSubItem.Name.IndexOf("_") + 1)))
                        {
                            GestioniScelte.Remove(registryOwner);
                            break;
                        }
                    foreach (MenuItem menuGestioni in MenuGestioni)
                        menuGestioni.IsEnabled = true;
                    foreach (MenuItem menuItemConti in MenuConti)
                    {
                        menuItemConti.IsChecked = false;
                        menuItemConti.IsEnabled = true;
                    }
                    ContiScelti.Clear();
                    mainGrid.Children.Clear();
                    break;
                case ("Conti"):
                    foreach (RegistryLocation registryLocation in ContiScelti)
                        if (registryLocation.IdLocation == Convert.ToInt16(menuSubItem.Name.Substring(menuSubItem.Name.IndexOf("_") + 1)))
                        {
                            ContiScelti.Remove(registryLocation);
                            break;
                        }
                    foreach (MenuItem menuItem in MenuConti)
                        menuItem.IsEnabled = true;
                    mainGrid.Children.Clear();
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
