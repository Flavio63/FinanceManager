using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerReportsViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerReportServices _reportServices;
        public ICommand CloseMeCommand { get; set; }
        private IList<int> _selectedOwners = new List<int>();
        private IList<int> _selectedYears = new List<int>();
        private IList<int> _selectedCurrency = new List<int>();

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
                SelectedOwner = new ObservableCollection<RegistryOwner>();
                CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                AvailableYears = _reportServices.GetAvailableYears();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ManagerReportsView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox LB = sender as ListBox;
                ComboBox CB = sender as ComboBox;
                if (LB != null)
                {
                    if (LB.Name == "ListOwners")
                    {
                        _selectedOwners.Clear();
                        foreach (RegistryOwner item in LB.SelectedItems)
                        {
                            _selectedOwners.Add(item.IdOwner);
                        }
                    }
                    if (LB.Name == "ListYears")
                    {
                        _selectedYears.Clear();
                        foreach (int y in LB.SelectedItems)
                        {
                            _selectedYears.Add(y);
                        }
                    }
                }
                if (CB != null)
                {

                }
            }
        }

        public void IsChecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            switch (radioButton.Content)
            {
                case "Singole Valute":
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "listCurrency", Visibility.Visible);
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "stackpanelCurrency", Visibility.Collapsed);
                    break;
                case "Tutto in Euro":
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "listCurrency", Visibility.Collapsed);
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "stackpanelCurrency", Visibility.Visible);
                    break;
                default:
                    break;
            }
        }
        #endregion events

        #region collection
        public ObservableCollection<RegistryOwner> OwnerList
        {
            get { return GetValue(() => OwnerList); }
            set { SetValue(() => OwnerList, value); }
        }
        public ObservableCollection<RegistryCurrency> CurrencyList
        {
            get { return GetValue(() => CurrencyList); }
            set { SetValue(() => CurrencyList, value); }
        }
        public ObservableCollection<RegistryOwner> SelectedOwner
        {
            get { return GetValue(() => SelectedOwner); }
            set { SetValue(() => SelectedOwner, value); }
        }
        public IList<int> AvailableYears
        {
            get { return GetValue(() => AvailableYears); }
            set { SetValue(() => AvailableYears, value); }
        }
        #endregion collection

        #region command
        public void CloseMe(object param)
        {
            ManagerReportsView MRV = param as ManagerReportsView;
            DockPanel wp = MRV.Parent as DockPanel;
            wp.Children.Remove(MRV);
        }
        #endregion command
    }
}
