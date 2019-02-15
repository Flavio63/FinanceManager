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
        public ICommand ClearCommand { get; set; }

        private IList<int> _selectedOwners;
        private double[] exchangeValue;
        private bool AllSetted = false;

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            CloseMeCommand = new CommandHandler(CloseMe);
            ViewCommand = new CommandHandler(ViewReport, CanDoReport);
            ClearCommand = new CommandHandler(ClearReport, CanClearReport);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = _services.GetGestioneList();
                _selectedOwners = new List<int>();
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
                            _selectedOwners.Add(item.Id_gestione);
                        break;
                    case "ListYears":
                        SelectedYears.Clear();
                        foreach (int y in LB.SelectedItems)
                            SelectedYears.Add(y);
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
                    break;
                case "Tutto in Euro":
                    exchangeValue = new double[3] { 0, 0, 0};
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "listCurrency", Visibility.Collapsed);
                    EnableControl.VisibleControlInGrid(((StackPanel)radioButton.Parent).Parent as Grid, "stackpanelCurrency", Visibility.Visible);
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
                        break;
                    case "exchangePound":
                        break;
                    case "exchangeFranchi":
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

        #region collection

        public RegistryOwnersList OwnerList
        {
            get { return GetValue(() => OwnerList); }
            set { SetValue(() => OwnerList, value); }
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
            Grid grid = param as Grid;
            if (_selectedOwners.Count() > 0 && SelectedYears.Count() > 0)
                return true;
            return false;
        }

        public bool CanClearReport(object param)
        {
            //Grid grid = param as Grid;
            //if (grid.RowDefinitions.Count > 0)
            //    return true;
            return false;
        }

        public void ClearReport(object param)
        {
            Grid grid = param as Grid;
            grid.Children.Clear();
            for (int r = grid.RowDefinitions.Count - 1; r >= 0; r--)
            {
                grid.RowDefinitions.RemoveAt(r);
            }
            SetUpViewModel();
        }

        public void ViewReport(object param)
        {
            Grid grid = param as Grid;
            //ReportProfitLosses = _reportServices.GetReport1(_selectedOwners, SelectedYears);
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
