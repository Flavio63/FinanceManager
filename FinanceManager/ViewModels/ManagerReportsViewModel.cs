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
        public ICommand CloseMeCommand { get; set; }
        private IList<int> selectedOwners = new List<int>();

        public ManagerReportsViewModel(IRegistryServices registryServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no services");
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
                ComboBox CB = sender as ComboBox;
                ListBox LB = sender as ListBox;
                if (LB != null)
                {
                    selectedOwners.Clear();
                    foreach (RegistryOwner item in LB.SelectedItems)
                    {
                        selectedOwners.Add(item.IdOwner);
                    }
                }
            }
        }

        public void IsChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            switch (checkBox.Name)
            {
                case "AllOwners":

                    break;
                case "AllYears":

                    break;
                case "AllInEuro":
                    if (checkBox.IsChecked == true)
                    {
                        EnableControl.EnableControlInGrid(checkBox.Parent as Grid, "cbCurrency", false);
                        EnableControl.EnableControlInGrid(checkBox.Parent as Grid, "spCurrency", true);
                    }
                    else
                    {
                        EnableControl.EnableControlInGrid(checkBox.Parent as Grid, "cbCurrency", true);
                        EnableControl.EnableControlInGrid(checkBox.Parent as Grid, "spCurrency", false);
                    }
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
