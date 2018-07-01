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
        public ICommand ViewCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        private IList<int> _selectedOwners = new List<int>();
        private IList<int> _selectedCurrency = new List<int>();
        private IList<string> _descCurrency = new List<string>();
        private double[] exchangeValue;
        private bool AllSetted = false;

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            CloseMeCommand = new CommandHandler(CloseMe);
            ViewCommand = new CommandHandler(ViewReport, CanDoReport);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = _services.GetRegistryOwners();
                CurrencyList = _services.GetRegistryCurrencyList();
                AvailableYears = _reportServices.GetAvailableYears();
                SelectedYears = new List<int>();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ManagerReportsView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox LB = sender as ListBox;
            if (LB != null)
            {
                switch (LB.Name)
                {
                    case "ListOwners":
                        _selectedOwners.Clear();
                        foreach (RegistryOwner item in LB.SelectedItems)
                            _selectedOwners.Add(item.IdOwner);
                        break;
                    case "ListYears":
                        SelectedYears.Clear();
                        foreach (int y in LB.SelectedItems)
                            SelectedYears.Add(y);
                        break;
                    case "listCurrency":
                        _selectedCurrency.Clear();
                        _descCurrency.Clear();
                        foreach (RegistryCurrency item in LB.SelectedItems)
                        {
                            _selectedCurrency.Add(item.IdCurrency);
                            _descCurrency.Add(item.CodeCurrency);
                        }
                        break;
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
                    exchangeValue = null;
                    AllSetted = false;
                    ExchangeDollar = 0;
                    ExchangePound = 0;
                    ExchangeFranchi = 0;
                    break;
                case "Tutto in Euro":
                    _descCurrency = new List<string>();
                    exchangeValue = new double[3] { 0, 0, 0};
                    _descCurrency.Add("EUR"); _descCurrency.Add("USD"); _descCurrency.Add("GBP"); _descCurrency.Add("CHF");
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "listCurrency", Visibility.Collapsed);
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "stackpanelCurrency", Visibility.Visible);
                    _selectedCurrency = new List<int>();
                    break;
                default:
                    break;
            }
        }

        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            double converted;
            if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out converted))
            {
                switch (TB.Name)
                {
                    case "exchangeDollar":
                        ExchangeDollar = Convert.ToDouble(TB.Text);
                        exchangeValue[0] = ExchangeDollar;
                        break;
                    case "exchangePound":
                        ExchangePound = Convert.ToDouble(TB.Text);
                        exchangeValue[1] = ExchangePound;
                        break;
                    case "exchangeFranchi":
                        ExchangeFranchi = Convert.ToDouble(TB.Text);
                        exchangeValue[2] = ExchangeFranchi;
                        break;
                }
            }
            for (int i = 0; i < exchangeValue.Count(); i++)
            {
                if (exchangeValue[i] == 0)
                {
                    AllSetted = false;
                    break;
                }
                AllSetted = true;
            }
        }
        #endregion events

        public double ExchangeDollar
        {
            get { return GetValue(() => ExchangeDollar); }
            set { SetValue(() => ExchangeDollar, value); }
        }

        public double ExchangePound
        {
            get { return GetValue(() => ExchangePound); }
            set { SetValue(() => ExchangePound, value); }
        }

        public double ExchangeFranchi
        {
            get { return GetValue(() => ExchangeFranchi); }
            set { SetValue(() => ExchangeFranchi, value); }
        }


        #region collection
        public RegistryOwnersList OwnerList
        {
            get { return GetValue(() => OwnerList); }
            set { SetValue(() => OwnerList, value); }
        }
        public RegistryCurrencyList CurrencyList
        {
            get { return GetValue(() => CurrencyList); }
            set { SetValue(() => CurrencyList, value); }
        }
        public RegistryOwnersList SelectedOwner
        {
            get { return GetValue(() => SelectedOwner); }
            set { SetValue(() => SelectedOwner, value); }
        }
        public IList<int> AvailableYears
        {
            get { return GetValue(() => AvailableYears); }
            set { SetValue(() => AvailableYears, value); }
        }

        public ReportProfitLossList ReportProfitLosses
        {
            get { return GetValue(() => ReportProfitLosses); }
            set { SetValue(() => ReportProfitLosses, value); }
        }

        public IList<int> SelectedYears
        {
            get { return GetValue(() => SelectedYears); }
            set { SetValue(() => SelectedYears, value); }
        }

        #endregion collection

        #region command
        public void CloseMe(object param)
        {
            ManagerReportsView MRV = param as ManagerReportsView;
            DockPanel wp = MRV.Parent as DockPanel;
            wp.Children.Remove(MRV);
        }

        public bool CanDoReport(object param)
        {
            if (_selectedOwners.Count() > 0 && SelectedYears.Count() > 0 && (_selectedCurrency.Count() > 0 || AllSetted))
                return true;
            return false;
        }

        public void ViewReport(object param)
        {
            Grid grid = param as Grid;
            ReportProfitLosses = _reportServices.GetReport1(_selectedOwners, SelectedYears, _selectedCurrency, AllSetted == true ? exchangeValue : null);
            ReportTrendAnno reportTrendAnno;
            ReportGuadagniView reportGuadagniView;
            foreach (int year in SelectedYears)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                reportTrendAnno = new ReportTrendAnno();
                reportTrendAnno.Desc_Anno = year.ToString();
                foreach (ReportProfitLoss item in ReportProfitLosses)
                {
                    if (year == item.Anno)
                    {
                        reportGuadagniView = new ReportGuadagniView(item);
                        reportTrendAnno.yearGrid.RowDefinitions.Add(new RowDefinition());
                        Grid.SetRow(reportGuadagniView, reportTrendAnno.yearGrid.RowDefinitions.Count - 1);
                        Grid.SetColumn(reportGuadagniView, 1);
                        reportTrendAnno.yearGrid.Children.Add(reportGuadagniView);
                    }
                }
                Grid.SetRow(reportTrendAnno, grid.RowDefinitions.Count - 1);
                grid.Children.Add(reportTrendAnno);
            }
        }

        #endregion command

    }
}
